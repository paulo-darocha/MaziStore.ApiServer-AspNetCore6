using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Infrastructure.Web.SmartTable;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   [ApiController]
   [Area("Catalog")]
   [Route("api/[controller]")]
   public class ProductAdmController : ControllerBase
   {
      private readonly IMediaService _mediaService;
      private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
      private readonly IRepository<ProductCategory> _productCategoryRepository;
      private readonly IRepository<ProductLink> _productLinkRepository;
      private readonly IRepository<ProductOptionValue> _productOptionValueRepository;
      private readonly IRepository<Product> _productRepository;
      private readonly IRepository<ProductMedia> _productMediaRepository;
      private readonly UserManager<User> _userManager;

      public ProductAdmController(
         IMediaService mediaService,
         IRepository<ProductAttributeValue> productAttributeValueRepository,
         IRepository<ProductCategory> productCategoryRepository,
         IRepository<ProductLink> productLinkRepository,
         IRepository<ProductOptionValue> productOptionValueRepository,
         IRepository<Product> productRepository,
         IRepository<ProductMedia> productMediaRepository,
         UserManager<User> userManager
      )
      {
         _mediaService = mediaService;
         _productAttributeValueRepository = productAttributeValueRepository;
         _productCategoryRepository = productCategoryRepository;
         _productLinkRepository = productLinkRepository;
         _productOptionValueRepository = productOptionValueRepository;
         _productRepository = productRepository;
         _productMediaRepository = productMediaRepository;
         _userManager = userManager;
      }

      [HttpPost]
      public async Task<IActionResult> List([FromBody] SmartTableParam param)
      {
         var query = _productRepository.QueryRp().Where(x => !x.IsDeleted);
         User currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);

         if (User.IsInRole("admin"))
         {
            query = query.Where(x => x.VendorId == currentUser.VendorId);
         }

         if (param.Search.PredicateObject != null)
         {
            dynamic search = param.Search.PredicateObject;
            if (search.Name != null)
            {
               string name = search.Name;
               query = query.Where(x => x.Name.Contains(name));
            }

            if (search.HasOptions != null)
            {
               bool hasOptions = search.HasOptions;
               query = query.Where(x => x.HasOptions == hasOptions);
            }

            if (search.IsVisibleIndividually != null)
            {
               bool isVisibleIndividually = search.IsVisibleIndividually;
               query = query.Where(
                  x => x.IsVisibleIndividually == isVisibleIndividually
               );
            }

            if (search.IsPublished != null)
            {
               bool isPublished = search.IsPublished;
               query = query.Where(x => x.IsPublished == isPublished);
            }

            if (search.CreatedOn != null)
            {
               if (search.CreatedOn.before != null)
               {
                  DateTimeOffset before = search.CreatedOn.before;
                  query = query.Where(x => x.CreatedOn <= before);
               }

               if (search.CreatedOn.after != null)
               {
                  DateTimeOffset after = search.CreatedOn.after;
                  query = query.Where(x => x.CreatedOn >= after);
               }
            }
         }

         var gridData = query.ToSmartTableResult(
            param,
            x =>
               new ProductListItem
               {
                  Id = x.Id,
                  Name = x.Name,
                  HasOptions = x.HasOptions,
                  IsVisibleIndividually = x.IsVisibleIndividually,
                  IsFeatured = x.IsFeatured,
                  IsAllowToOrder = x.IsAllowToOrder,
                  IsCallForPricing = x.IsCallForPricing,
                  StockQuantity = x.StockQuantity,
                  CreatedOn = x.CreatedOn,
                  IsPublished = x.IsPublished
               }
         );

         return Ok(gridData);
      }

      [HttpGet("{id}")]
      public async Task<IActionResult> Get(long id)
      {
         var product = _productRepository
            .QueryRp()
            .Include(x => x.ThumbnailImage)
            .Include(x => x.Medias)
            .ThenInclude(y => y.Media)
            .Include(x => x.ProductLinks)
            .ThenInclude(y => y.LinkedProduct)
            .ThenInclude(z => z.ThumbnailImage)
            .Include(x => x.OptionValues)
            .ThenInclude(y => y.Option)
            .Include(x => x.AttributeValues)
            .ThenInclude(y => y.Attribute)
            .ThenInclude(z => z.Group)
            .FirstOrDefault(x => x.Id == id);

         var currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);

         if (!User.IsInRole("admin") && product.VendorId != currentUser.VendorId)
         {
            return BadRequest("You do not have permission to manage this product.");
         }

         var productVm = new ProductVm
         {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            MetaTitle = product.MetaTitle,
            MetaKeywords = product.MetaKeywords,
            MetaDescription = product.MetaDescription,
            Sku = product.Sku,
            Gtin = product.Gtin,
            ShortDescription = product.ShortDescription,
            Description = product.Description,
            Specification = product.Specification,
            OldPrice = product.OldPrice,
            Price = product.Price,
            SpecialPrice = product.SpecialPrice,
            SpecialPriceStart = product.SpecialPriceStart,
            SpecialPriceEnd = product.SpecialPriceEnd,
            IsFeatured = product.IsFeatured,
            IsPublished = product.IsPublished,
            IsCallForPricing = product.IsCallForPricing,
            IsAllowToOrder = product.IsAllowToOrder,
            CategoryIds = product.Categories.Select(x => x.CategoryId).ToList(),
            ThumbnailImageUrl = _mediaService.GetThumbnailUrl(
               product.ThumbnailImage
            ),
            BrandId = product.BrandId,
            TaxClassId = product.TaxClassId,
            StockTrackingIsEnabled = product.StockTrackingIsEnabled
         };

         foreach (
            var productMedia in product.Medias.Where(
               x => x.Media.MediaType == MediaType.Image
            )
         )
         {
            productVm.ProductImages.Add(
               new ProductMediaVm
               {
                  Id = productMedia.Id,
                  MediaUrl = _mediaService.GetThumbnailUrl(productMedia.Media)
               }
            );
         }

         foreach (
            var productMedia in product.Medias.Where(
               x => x.Media.MediaType == MediaType.File
            )
         )
         {
            productVm.ProductDocuments.Add(
               new ProductMediaVm
               {
                  Id = product.Id,
                  Caption = productMedia.Media.Caption,
                  MediaUrl = _mediaService.GetMediaUrl(productMedia.Media)
               }
            );
         }

         productVm.Options = product.OptionValues
            .OrderBy(x => x.SortIndex)
            .Select(
               x =>
                  new ProductOptionVm
                  {
                     Id = x.OptionId,
                     Name = x.Option.Name,
                     DisplayType = x.DisplayType,
                     Values = JsonSerializer.Deserialize<
                        IList<ProductOptionValueVm>
                     >(x.Value)
                  }
            )
            .ToList();

         foreach (
            var variation in product.ProductLinks
               .Where(x => x.LinkType == ProductLinkType.Super)
               .Select(x => x.LinkedProduct)
               .Where(x => !x.IsDeleted)
               .OrderBy(x => x.Id)
         )
         {
            productVm.Variations.Add(
               new ProductVariationVm
               {
                  Id = variation.Id,
                  Name = variation.Name,
                  Sku = variation.Sku,
                  Gtin = variation.Gtin,
                  Price = variation.Price,
                  OldPrice = variation.OldPrice,
                  NormalizedName = variation.NormalizedName,
                  ThumbnailImageUrl = _mediaService.GetMediaUrl(
                     variation.ThumbnailImage
                  ),
                  ImageUrls = GetProductImageUrls(variation.Id).ToList(),
                  OptionCombinations = variation.OptionCombinations
                     .Select(
                        x =>
                           new ProductOptionCombinationVm
                           {
                              OptionId = x.OptionId,
                              OptionName = x.Option.Name,
                              Value = x.Value,
                              SortIndex = x.SortIndex
                           }
                     )
                     .OrderBy(x => x.SortIndex)
                     .ToList()
               }
            );
         }

         foreach (
            var relatedProduct in product.ProductLinks
               .Where(x => x.LinkType == ProductLinkType.Related)
               .Select(x => x.LinkedProduct)
               .Where(x => !x.IsDeleted)
               .OrderBy(x => x.Id)
         )
         {
            productVm.RelatedProducts.Add(
               new ProductLinkVm
               {
                  Id = relatedProduct.Id,
                  Name = relatedProduct.Name,
                  IsPublished = relatedProduct.IsPublished
               }
            );
         }

         foreach (
            var crossSellProduct in product.ProductLinks
               .Where(x => x.LinkType == ProductLinkType.CrossSell)
               .Select(x => x.LinkedProduct)
               .Where(x => !x.IsDeleted)
               .OrderBy(x => x.Id)
         )
         {
            productVm.CrossSellProducts.Add(
               new ProductLinkVm
               {
                  Id = crossSellProduct.Id,
                  Name = crossSellProduct.Name,
                  IsPublished = crossSellProduct.IsPublished
               }
            );
         }

         productVm.Attributes = product.AttributeValues
            .Select(
               x =>
                  new ProductAttributeVm
                  {
                     AttributeValueId = x.Id,
                     Id = x.AttributeId,
                     Name = x.Attribute.Name,
                     GroupName = x.Attribute.Group.Name,
                     Value = x.Value
                  }
            )
            .ToList();

         return Ok(productVm);
      }

      // ////////////////////////////////////////////////////

      private IEnumerable<string> GetProductImageUrls(long productId)
      {
         var imageUrls = _productMediaRepository
            .QueryRp()
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.Id)
            .Select(x => x.Media)
            .ToList()
            .Select(x => _mediaService.GetMediaUrl(x));

         return imageUrls;
      }
   }
}
