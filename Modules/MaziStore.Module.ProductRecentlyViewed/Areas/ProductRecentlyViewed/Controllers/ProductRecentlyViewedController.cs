using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Catalog.Services;
using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Core.Services;
using MaziStore.Module.ProductRecentlyViewed.Areas.ProductRecentlyViewed.ViewModels;
using MaziStore.Module.ProductRecentlyViewed.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.ProductRecentlyViewed.Areas.ProductRecentlyViewed.Controllers
{
   [ApiController]
   [Area("ProductRecentlyViewed")]
   [Route("api/[controller]")]
   public class ProductRecentlyViewedController : ControllerBase
   {
      private readonly IRecentlyViewedProductRepository _rVproductRepository;
      private readonly IMediaService _mediaService;
      private readonly IProductPricingService _productPricingProduct;
      private readonly IWorkContext _workContext;

      public ProductRecentlyViewedController(
         IRecentlyViewedProductRepository rVproductRepository,
         IMediaService mediaService,
         IProductPricingService productPricingProduct,
         IWorkContext workContext
      )
      {
         _rVproductRepository = rVproductRepository;
         _mediaService = mediaService;
         _productPricingProduct = productPricingProduct;
         _workContext = workContext;
      }

      [HttpPost("{id?}")]
      public async Task<IActionResult> GetProductsRecentlyViewed(
         [FromBody] ProductsRecentlyViewedVm model,
         long id = 0
      )
      {
         if (id == 0)
         {
            id = (await _workContext.GetCurrentUser()).Id;
         }
         IQueryable<Product> query = _rVproductRepository
            .GetRecentlyViewedProduct(id)
            .Include(x => x.ThumbnailImage);

         if (model.ProductId.HasValue)
         {
            query = query.Where(x => x.Id != model.ProductId.Value);
         }

         var result = query
            .Take(model.ItemCount)
            .Select(x => ProductThumbnail.FromProduct(x))
            .ToList();

         foreach (var product in result)
         {
            product.ThumbnailUrl = _mediaService.GetThumbnailUrl(
               product.ThumbnailImage
            );
            product.CalculatedProductPrice =
               _productPricingProduct.CalculateProductPrice(product);
         }

         return Ok(result);
      }
   }
}
