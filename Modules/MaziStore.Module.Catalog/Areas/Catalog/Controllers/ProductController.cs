using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Catalog.Services;
using MaziStore.Module.Core.Events;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [ApiController]
   [Area("Catalog")]
   [Route("api/[controller]")]
   public class ProductController : ControllerBase
   {
      private readonly IMediaService _mediaService;
      private readonly IRepository<Product> _productRepository;
      private readonly IMediator _mediator;
      private readonly IProductPricingService _productPricingService;

      public ProductController(
         IMediaService mediaService,
         IRepository<Product> productRepository,
         IMediator mediator,
         IProductPricingService productPricingService
      )
      {
         _mediaService = mediaService;
         _productRepository = productRepository;
         _mediator = mediator;
         _productPricingService = productPricingService;
      }

      [HttpGet("{id}")]
      public async Task<IActionResult> ProductDetail(long id)
      {
         var product = _productRepository
            .QueryRp()
            .Include(x => x.OptionValues)
            .Include(x => x.Categories)
            .ThenInclude(y => y.Category)
            .Include(x => x.AttributeValues)
            .ThenInclude(y => y.Attribute)
            .Include(x => x.ProductLinks)
            .ThenInclude(y => y.LinkedProduct)
            .ThenInclude(z => z.ThumbnailImage)
            .Include(x => x.ThumbnailImage)
            .Include(x => x.Medias)
            .ThenInclude(y => y.Media)
            .Include(x => x.Brand)
            .FirstOrDefault(x => x.Id == id && x.IsPublished);

         if (product == null)
         {
            return NotFound($"Product - id '{id}' - not found");
         }

         var model = new ProductDetail
         {
            Id = product.Id,
            Name = product.Name,
            Brand = product.Brand,
            CalculatedProductPrice = _productPricingService.CalculateProductPrice(
               product
            ),
            IsCallForPricing = product.IsCallForPricing,
            IsAllowToOrder = product.IsAllowToOrder,
            StockTrackingIsEnabled = product.StockTrackingIsEnabled,
            StockQuantity = product.StockQuantity,
            ShortDescription = product.ShortDescription,
            MetaTitle = product.MetaTitle,
            MetaKeywords = product.MetaKeywords,
            MetaDescription = product.MetaDescription,
            Description = product.Description,
            Specification = product.Specification,
            ReviewsCount = product.ReviewsCount,
            RatingAverage = product.RatingAverage,
            Attributes = product.AttributeValues
               .Select(
                  x =>
                     new ProductDetailAttribute
                     {
                        Name = x.Attribute.Name,
                        Value = x.Value
                     }
               )
               .ToList(),
            Categories = product.Categories
               .Select(
                  x =>
                     new ProductDetailCategory
                     {
                        Id = x.CategoryId,
                        Name = x.Category.Name,
                        Slug = x.Category.Slug
                     }
               )
               .ToList()
         };

         MapProductVariantToProductVm(product, model);

         MapRelatedProductToProductVm(product, model);

         MapProductOptionToProductVm(product, model);

         MapProductImagesToProductVm(product, model);

         await _mediator.Publish(
            new EntityViewed { EntityId = product.Id, EntityTypeId = "Product" }
         );
         _productRepository.SaveChangesRp();

         return Ok(model);
      }

      // ///////////////////////////////////////////////

      private void MapProductVariantToProductVm(
         Product product,
         ProductDetail model
      )
      {
         if (!product.ProductLinks.Any(x => x.LinkType == ProductLinkType.Super))
         {
            return;
         }

         var variations = _productRepository
            .QueryRp()
            .Include(x => x.OptionCombinations)
            .ThenInclude(y => y.Option)
            .Include(x => x.Medias)
            .ThenInclude(y => y.Media)
            .Where(
               x =>
                  x.LinkedProductLinks.Any(
                     y =>
                        y.ProductId == product.Id
                        && y.LinkType == ProductLinkType.Super
                  )
            )
            .Where(x => x.IsPublished)
            .ToList();

         foreach (var variation in variations)
         {
            var variationVm = new ProductDetailVariation
            {
               Id = variation.Id,
               Name = variation.Name,
               NormalizedName = variation.NormalizedName,
               IsAllowToOrder = variation.IsAllowToOrder,
               IsCallForPricing = variation.IsCallForPricing,
               StockTrackingIsEnabled = variation.StockTrackingIsEnabled,
               StockQuantity = variation.StockQuantity,
               CalculatedProductPrice =
                  _productPricingService.CalculateProductPrice(variation),
               Images = variation.Medias
                  .Where(x => x.Media.MediaType == MediaType.Image)
                  .Select(
                     y =>
                        new MediaViewModel
                        {
                           Url = _mediaService.GetMediaUrl(y.Media),
                           ThumbnailUrl = _mediaService.GetThumbnailUrl(y.Media)
                        }
                  )
                  .ToList()
            };

            var optionCombinations = variation.OptionCombinations.OrderBy(
               x => x.SortIndex
            );
            foreach (var combination in optionCombinations)
            {
               variationVm.Options.Add(
                  new ProductDetailVariationOption
                  {
                     OptionId = combination.OptionId,
                     OptionName = combination.Option.Name,
                     Value = combination.Value
                  }
               );
            }

            model.Variations.Add(variationVm);
         }
      }

      private void MapRelatedProductToProductVm(
         Product product,
         ProductDetail model
      )
      {
         var publishedProductLinks = product.ProductLinks.Where(
            x =>
               x.LinkedProduct.IsPublished
               && (
                  x.LinkType == ProductLinkType.Related
                  || x.LinkType == ProductLinkType.CrossSell
               )
         );

         foreach (var productLink in publishedProductLinks)
         {
            var linkedProduct = productLink.LinkedProduct;
            var productThumbnail = ProductThumbnail.FromProduct(linkedProduct);
            productThumbnail.ThumbnailUrl = _mediaService.GetThumbnailUrl(
               linkedProduct.ThumbnailImage
            );
            productThumbnail.CalculatedProductPrice =
               _productPricingService.CalculateProductPrice(linkedProduct);

            if (productLink.LinkType == ProductLinkType.Related)
            {
               model.RelatedProducts.Add(productThumbnail);
            }

            if (productLink.LinkType == ProductLinkType.CrossSell)
            {
               model.CrossSellProducts.Add(productThumbnail);
            }
         }
      }

      private void MapProductOptionToProductVm(Product product, ProductDetail model)
      {
         foreach (var item in product.OptionValues)
         {
            var optionValues = JsonSerializer.Deserialize<
               IList<ProductOptionValueVm>
            >(item.Value);
            foreach (var value in optionValues)
            {
               if (!model.OptionDisplayValues.ContainsKey(value.Key))
               {
                  model.OptionDisplayValues.Add(
                     value.Key,
                     new ProductOptionDisplay
                     {
                        DisplayType = item.DisplayType,
                        Value = value.Display
                     }
                  );
               }
            }
         }
      }

      private void MapProductImagesToProductVm(Product product, ProductDetail model)
      {
         model.Images = product.Medias
            .Where(x => x.Media.MediaType == MediaType.Image)
            .Select(
               x =>
                  new MediaViewModel
                  {
                     Url = _mediaService.GetMediaUrl(x.Media),
                     ThumbnailUrl = _mediaService.GetThumbnailUrl(x.Media)
                  }
            )
            .ToList();
      }
   }
}
