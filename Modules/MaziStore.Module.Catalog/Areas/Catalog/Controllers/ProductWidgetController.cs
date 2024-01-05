using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Catalog.Services;
using MaziStore.Module.Core.Areas.Core.ViewModels;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [ApiController]
   [Area("Catalog")]
   [Route("api/[controller]")]
   public class ProductWidgetController : ControllerBase
   {
      private readonly IRepository<Product> _productRepository;
      private readonly IMediaService _mediaService;
      private readonly IProductPricingService _productPricingPrice;

      public ProductWidgetController(
         IRepository<Product> productRepository,
         IMediaService mediaService,
         IProductPricingService productPricingPrice
      )
      {
         _productRepository = productRepository;
         _mediaService = mediaService;
         _productPricingPrice = productPricingPrice;
      }

      [HttpPost]
      public IActionResult GetProductWidgets(
         [FromBody] WidgetInstanceViewModel widgetInstances
      )
      {
         var model = new ProductWidgetComponentVm
         {
            Id = widgetInstances.Id,
            WidgetName = widgetInstances.Name,
            Setting = JsonSerializer.Deserialize<ProductWidgetSetting>(
               widgetInstances.Data
            )
         };

         var query = _productRepository
            .QueryRp()
            .Where(x => x.IsPublished && x.IsVisibleIndividually);
         if (
            model.Setting.CategoryId.HasValue && model.Setting.CategoryId.Value > 0
         )
         {
            query = query.Where(
               x =>
                  x.Categories.Any(
                     y => y.CategoryId == model.Setting.CategoryId.Value
                  )
            );
         }

         if (model.Setting.FeaturedOnly)
         {
            query = query.Where(x => x.IsFeatured);
         }

         model.Products = query
            .Include(x => x.ThumbnailImage)
            .OrderByDescending(x => x.CreatedOn)
            .Take(model.Setting.NumberOfProducts)
            .Select(x => ProductThumbnail.FromProduct(x))
            .ToList();

         foreach (var product in model.Products)
         {
            product.ThumbnailUrl = _mediaService.GetThumbnailUrl(
               product.ThumbnailImage
            );
            product.CalculatedProductPrice =
               _productPricingPrice.CalculateProductPrice(product);
         }

         return Ok(model);
      }
   }
}
