using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [ApiController]
   [Area("Catalog")]
   [Route("api/[controller]")]
   public class CategoryMenuController : ControllerBase
   {
      private readonly IRepository<Category> _categoryRepository;

      public CategoryMenuController(IRepository<Category> categoryRepository)
      {
         _categoryRepository = categoryRepository;
      }

      [HttpGet]
      public IActionResult GetCategoriesMenu()
      {
         var categories = _categoryRepository
            .QueryRp()
            .Where(x => !x.IsDeleted && x.IncludeInMenu)
            .ToList();

         var categoryMenuItems = new List<CategoryMenuItem>();
         var topCategories = categories
            .Where(x => !x.ParentId.HasValue)
            .OrderByDescending(x => x.DisplayOrder);

         foreach (var category in topCategories)
         {
            var categoryMenuItem = Map(category);
            categoryMenuItems.Add(categoryMenuItem);
         }

         foreach (var item in categoryMenuItems)
         {
            if (item.Parent is not null)
            {
               item.Parent.Parent = null;
               item.Parent.ChildItems = null;
            }
            foreach (var subItem in item.ChildItems)
            {
               subItem.ChildItems = null;
            }
         }

         return Ok(categoryMenuItems);
      }

      // //////////////////////////////////////////////////////

      private CategoryMenuItem Map(Category category)
      {
         var categoryMenuItem = new CategoryMenuItem
         {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Name
         };

         var childCategories = category.Children;
         foreach (
            var childCategory in childCategories.OrderByDescending(
               x => x.DisplayOrder
            )
         )
         {
            var childCategoryMenuItem = Map(childCategory);
            categoryMenuItem.AddChildItem(childCategoryMenuItem);
         }

         return categoryMenuItem;
      }
   }
}
