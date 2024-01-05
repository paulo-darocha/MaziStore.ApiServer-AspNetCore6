using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Catalog.Services;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [ApiController]
   [Area("Catalog")]
   [Route("api/[controller]")]
   public class CategoryController : ControllerBase
   {
      private readonly IRepository<Category> _categoryRepository;
      private readonly IMediaService _mediaService;
      private readonly IRepository<Product> _productRepository;
      private readonly IRepository<Brand> _brandRepository;
      private readonly IProductPricingService _productPricingService;
      private int _pageSize;

      public CategoryController(
         IRepository<Category> categoryRepository,
         IMediaService mediaService,
         IRepository<Product> productRepository,
         IRepository<Brand> brandRepository,
         IProductPricingService productPricingService
      )
      {
         _categoryRepository = categoryRepository;
         _mediaService = mediaService;
         _productRepository = productRepository;
         _brandRepository = brandRepository;
         _productPricingService = productPricingService;
         _pageSize = 25;
      }

      [HttpPost("{id}")]
      public IActionResult CategoryDetail(
         [FromBody] SearchOption searchOption,
         long id
      )
      {
         var category = _categoryRepository
            .QueryRp()
            .FirstOrDefault(x => x.Id == id);

         if (category == null)
         {
            return BadRequest("Category not found.");
         }

         var model = new ProductsByCategory
         {
            CategoryId = category.Id,
            ParentCategoryId = category.ParentId,
            CategoryName = category.Name,
            CategorySlug = category.Slug,
            CategoryMetaTitle = category.MetaTitle,
            CategoryMetaKeywords = category.MetaKeywords,
            CategoryMetaDescription = category.MetaDescription,
            CurrentSearchOption = searchOption,
            FilterOption = new FilterOption()
         };

         var query = _productRepository
            .QueryRp()
            .Where(
               x =>
                  x.Categories.Any(y => y.CategoryId == category.Id)
                  && x.IsPublished
                  && x.IsVisibleIndividually
            );

         if (query.Count() == 0)
         {
            model.TotalProduct = 0;
            return Ok(model);
         }

         AppendFilterOptionsToModel(model, query);

         if (searchOption.MinPrice.HasValue)
         {
            query = query.Where(x => x.Price >= searchOption.MinPrice.Value);
         }

         if (searchOption.MaxPrice.HasValue)
         {
            query = query.Where(x => x.Price <= searchOption.MaxPrice.Value);
         }

         var categories = searchOption.GetCategories();
         if (categories.Any())
         {
            query = query.Where(
               p =>
                  p.Categories
                     .Select(x => x.CategoryId)
                     .Intersect(
                        _categoryRepository
                           .QueryRp()
                           .Where(y => categories.Contains(y.Slug))
                           .Select(z => z.Id)
                     )
                     .Any()
            );
         }

         var brands = searchOption.GetBrands().ToArray();
         if (brands.Any())
         {
            query = query.Where(
               x => x.BrandId != null && brands.Contains(x.Brand.Slug)
            );
         }

         model.TotalProduct = query.Count();

         var currentPageNum = searchOption.Page <= 0 ? 1 : searchOption.Page;
         var offset = (_pageSize * currentPageNum) - _pageSize;

         while (currentPageNum > 1 && offset >= model.TotalProduct)
         {
            currentPageNum--;
            offset = (_pageSize * currentPageNum) - _pageSize;
         }

         query = ApplySort(searchOption, query);

         var products = query
            .Include(x => x.ThumbnailImage)
            .Skip(offset)
            .Take(_pageSize)
            .Select(x => ProductThumbnail.FromProduct(x))
            .ToList();

         foreach (var product in products)
         {
            product.ThumbnailUrl = _mediaService.GetThumbnailUrl(
               product.ThumbnailImage
            );
            product.CalculatedProductPrice =
               _productPricingService.CalculateProductPrice(product);
         }

         model.Products = products;
         model.CurrentSearchOption.PageSize = _pageSize;
         model.CurrentSearchOption.Page = currentPageNum;

         return Ok(model);
      }

      // ///////////////////////////////////////////////////

      private void AppendFilterOptionsToModel(
         ProductsByCategory model,
         IQueryable<Product> query
      )
      {
         model.FilterOption.Price.MaxPrice = query.Max(x => x.Price);
         model.FilterOption.Price.MinPrice = query.Min(x => x.Price);

         model.FilterOption.Categories = query
            .SelectMany(x => x.Categories)
            .GroupBy(
               x =>
                  new
                  {
                     x.Category.Id,
                     x.Category.Name,
                     x.Category.Slug,
                     x.Category.ParentId
                  }
            )
            .Select(
               y =>
                  new FilterCategory
                  {
                     Id = (int)y.Key.Id,
                     Name = y.Key.Name,
                     Slug = y.Key.Slug,
                     ParentId = y.Key.ParentId,
                     Count = y.Count()
                  }
            )
            .ToList();

         model.FilterOption.Brands = query
            .Include(x => x.Brand)
            .Where(x => x.BrandId != null)
            .ToList()
            .GroupBy(x => x.Brand)
            .Select(
               y =>
                  new FilterBrand
                  {
                     Id = y.Key.Id,
                     Name = y.Key.Name,
                     Slug = y.Key.Slug,
                     Count = y.Count()
                  }
            )
            .ToList();
      }

      private static IQueryable<Product> ApplySort(
         SearchOption searchOption,
         IQueryable<Product> query
      )
      {
         var sortBy = searchOption.Sort ?? string.Empty;
         switch (sortBy.ToLower())
         {
            case "price-desc":
               query = query.OrderByDescending(x => x.Price);
               break;
            default:
               query = query.OrderBy(x => x.Price);
               break;
         }

         return query;
      }
   }
}
