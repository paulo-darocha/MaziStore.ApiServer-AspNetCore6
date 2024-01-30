using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Services
{
   public class CategoryService : ICategoryService
   {
      private const string CategoryEntityTypeId = "Category";
      private readonly IRepository<Category> _categoryRepository;
      private readonly IEntityService _entityService;

      public CategoryService(
         IRepository<Category> categoryRepository,
         IEntityService entityService
      )
      {
         _categoryRepository = categoryRepository;
         _entityService = entityService;
      }

      public async Task<IList<CategoryListItem>> GetAll()
      {
         var categories = await _categoryRepository
            .QueryRp()
            .Where(x => !x.IsDeleted)
            .ToListAsync();
         var categoriesList = new List<CategoryListItem>();

         foreach (var category in categories)
         {
            var categoryListItem = new CategoryListItem
            {
               Id = category.Id,
               IsPublished = category.IsPublished,
               IncludeInMenu = category.IncludeInMenu,
               Name = category.Name,
               DisplayOrder = category.DisplayOrder,
               ParentId = category.ParentId
            };

            var parentCategory = category.Parent;

            while (parentCategory != null)
            {
               categoryListItem.Name =
                  $"{parentCategory.Name} >> {categoryListItem.Name}";
               parentCategory = parentCategory.Parent;
            }

            categoriesList.Add(categoryListItem);
         }

         return categoriesList.OrderBy(x => x.Name).ToList();
      }

      public async Task Create(Category category)
      {
         using (var transaction = _categoryRepository.BeginTransactionRp())
         {
            category.Slug = _entityService.ToSafeSlug(
               category.Slug,
               category.Id,
               CategoryEntityTypeId
            );
            _categoryRepository.AddRp(category);
            await _categoryRepository.SaveChangesRpAsync();

            _entityService.Add(
               category.Name,
               category.Slug,
               category.Id,
               CategoryEntityTypeId
            );

            await _categoryRepository.SaveChangesRpAsync();

            transaction.Commit();
         }
      }

      public async Task Update(Category category)
      {
         category.Slug = _entityService.ToSafeSlug(
            category.Slug,
            category.Id,
            CategoryEntityTypeId
         );
         _entityService.Update(
            category.Name,
            category.Slug,
            category.Id,
            CategoryEntityTypeId
         );

         await _categoryRepository.SaveChangesRpAsync();
      }

      public async Task Delete(Category category)
      {
         await _entityService.Remove(category.Id, CategoryEntityTypeId);
         category.IsDeleted = true;
         _categoryRepository.SaveChangesRp();
      }
   }
}
