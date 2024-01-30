using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Catalog.Services;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Infrastructure.Helpers;
using MaziStore.Module.Infrastructure.Web.SmartTable;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
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
      private readonly IProductService _productService;
      private readonly IRepository<ProductMedia> _productMediaRepository;
      private readonly UserManager<User> _userManager;

      public ProductAdmController(
         IMediaService mediaService,
         IRepository<ProductAttributeValue> productAttributeValueRepository,
         IRepository<ProductCategory> productCategoryRepository,
         IRepository<ProductLink> productLinkRepository,
         IRepository<ProductOptionValue> productOptionValueRepository,
         IRepository<Product> productRepository,
         IProductService productService,
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
         _productService = productService;
         _productMediaRepository = productMediaRepository;
         _userManager = userManager;
      }

      [HttpPost("list")]
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
      public async Task<IActionResult> GetProduct(long id)
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

      [HttpPost]
      public async Task<IActionResult> Post([FromForm] ProductForm model)
      {
         MapUploadedFile(model);
         if (!ModelState.IsValid)
         {
            return BadRequest(ModelState);
         }
         var currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);

         var product = new Product
         {
            Name = model.Product.Name,
            Slug = model.Product.Slug,
            MetaTitle = model.Product.MetaTitle,
            MetaKeywords = model.Product.MetaKeywords,
            MetaDescription = model.Product.MetaDescription,
            Sku = model.Product.Sku,
            Gtin = model.Product.Gtin,
            ShortDescription = model.Product.ShortDescription,
            Description = model.Product.Description,
            Specification = model.Product.Specification,
            Price = model.Product.Price,
            OldPrice = model.Product.OldPrice,
            SpecialPrice = model.Product.SpecialPrice,
            SpecialPriceStart = model.Product.SpecialPriceStart,
            SpecialPriceEnd = model.Product.SpecialPriceEnd,
            IsPublished = model.Product.IsPublished,
            IsFeatured = model.Product.IsFeatured,
            IsCallForPricing = model.Product.IsCallForPricing,
            IsAllowToOrder = model.Product.IsAllowToOrder,
            BrandId = model.Product.BrandId,
            TaxClassId = model.Product.TaxClassId,
            StockTrackingIsEnabled = model.Product.StockTrackingIsEnabled,
            HasOptions = model.Product.Variations.Any() ? true : false,
            IsVisibleIndividually = true,
            CreatedBy = currentUser,
            LatestUpdatedBy = currentUser
         };

         if (!User.IsInRole("admin"))
         {
            product.VendorId = currentUser.VendorId;
         }

         var optionIndex = 0;
         foreach (var option in model.Product.Options)
         {
            product.AddOptionValue(
               new ProductOptionValue
               {
                  OptionId = option.Id,
                  DisplayType = option.DisplayType,
                  Value = JsonSerializer.Serialize(option.Values),
                  SortIndex = optionIndex
               }
            );
            optionIndex++;
         }

         foreach (var attribute in model.Product.Attributes)
         {
            var attributeValue = new ProductAttributeValue
            {
               AttributeId = attribute.Id,
               Value = attribute.Value
            };
            product.AddAttributeValue(attributeValue);
         }

         foreach (var categoryId in model.Product.CategoryIds)
         {
            var productCategory = new ProductCategory { CategoryId = categoryId };
            product.AddCategory(productCategory);
         }

         await SaveProductMedias(model, product);

         await MapProductVariationVmToProduct(currentUser, model, product);

         MapProductLinkVmToProduct(model, product);

         var productPriceHistory = CreatePriceHistory(currentUser, product);
         product.PriceHistories.Add(productPriceHistory);

         _productService.Create(product);
         return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, null);
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

      private void MapUploadedFile(ProductForm model)
      {
         foreach (var file in Request.Form.Files)
         {
            if (file.Name.Contains("productImages"))
            {
               model.ProductImages.Add(file);
            }
            else if (file.Name.Contains("productDocuments"))
            {
               model.ProductDocuments.Add(file);
            }
            else if (file.Name.Contains("product[variations]"))
            {
               var key = file.Name.Replace("product", "");
               var keyParts = key.Split(
                  new char[] { '[', ']' },
                  StringSplitOptions.RemoveEmptyEntries
               );
               var variantIndex = int.Parse(keyParts[1]);
               if (key.Contains("newImages"))
               {
                  model.Product.Variations[variantIndex].NewImages.Add(file);
               }
               else
               {
                  model.Product.Variations[variantIndex].ThumbnailImage = file;
               }
            }
         }
      }

      private async Task SaveProductMedias(ProductForm model, Product product)
      {
         if (model.ThumbnailImage != null)
         {
            var fileName = await SaveFile(model.ThumbnailImage);
            if (product.ThumbnailImage != null)
            {
               product.ThumbnailImage.FileName = fileName;
            }
            else
            {
               product.ThumbnailImage = new Media { FileName = fileName };
            }
         }

         foreach (var file in model.ProductImages)
         {
            var fileName = await SaveFile(file);
            var productMedia = new ProductMedia
            {
               Product = product,
               Media = new Media
               {
                  FileName = fileName,
                  MediaType = MediaType.Image
               }
            };
            product.AddMedia(productMedia);
         }

         foreach (var file in model.ProductDocuments)
         {
            var fileName = await SaveFile(file);
            var productMedia = new ProductMedia
            {
               Product = product,
               Media = new Media
               {
                  FileName = fileName,
                  MediaType = MediaType.File,
                  Caption = file.FileName
               }
            };
            product.AddMedia(productMedia);
         }
      }

      private async Task<string> SaveFile(IFormFile file)
      {
         var originalFileName = ContentDispositionHeaderValue
            .Parse(file.ContentDisposition)
            .FileName.Value.Trim('"');
         var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

         await _mediaService.SaveMediaAsync(
            file.OpenReadStream(),
            fileName,
            file.ContentType
         );

         return fileName;
      }

      private async Task MapProductVariationVmToProduct(
         User loginUser,
         ProductForm model,
         Product product
      )
      {
         foreach (var variationVm in model.Product.Variations)
         {
            var productLink = new ProductLink
            {
               LinkType = ProductLinkType.Super,
               Product = product,
               LinkedProduct = product.Clone()
            };

            productLink.LinkedProduct.CreatedById = loginUser.Id;
            productLink.LinkedProduct.LatestUpdatedById = loginUser.Id;
            productLink.LinkedProduct.Name = variationVm.Name;
            productLink.LinkedProduct.Slug = variationVm.Name.ToUrlFriendly();
            productLink.LinkedProduct.Sku = variationVm.Sku;
            productLink.LinkedProduct.Gtin = variationVm.Gtin;
            productLink.LinkedProduct.Price = variationVm.Price;
            productLink.LinkedProduct.OldPrice = variationVm.OldPrice;
            productLink.LinkedProduct.NormalizedName = variationVm.NormalizedName;
            productLink.LinkedProduct.HasOptions = false;
            productLink.LinkedProduct.IsVisibleIndividually = false;

            if (product.ThumbnailImage != null)
            {
               productLink.LinkedProduct.ThumbnailImage = new Media
               {
                  FileName = product.ThumbnailImage.FileName
               };
            }

            await MapProductVariantImageFromVm(
               variationVm,
               productLink.LinkedProduct
            );

            foreach (var combinationVm in variationVm.OptionCombinations)
            {
               productLink.LinkedProduct.AddOptionCombination(
                  new ProductOptionCombination
                  {
                     OptionId = combinationVm.OptionId,
                     Value = combinationVm.Value,
                     SortIndex = combinationVm.SortIndex
                  }
               );
            }

            var productPriceHistory = CreatePriceHistory(
               loginUser,
               productLink.LinkedProduct
            );
            product.PriceHistories.Add(productPriceHistory);

            product.AddProductLinks(productLink);
         }
      }

      private async Task MapProductVariantImageFromVm(
         ProductVariationVm variationVm,
         Product product
      )
      {
         if (variationVm.ThumbnailImage != null)
         {
            var thumbnailImageFileName = await SaveFile(variationVm.ThumbnailImage);
            if (product.ThumbnailImage != null)
            {
               product.ThumbnailImage.FileName = thumbnailImageFileName;
            }
            else
            {
               product.ThumbnailImage = new Media
               {
                  FileName = thumbnailImageFileName
               };
            }

            foreach (var image in variationVm.NewImages)
            {
               var fileName = await SaveFile(image);
               var productMedia = new ProductMedia
               {
                  Product = product,
                  Media = new Media
                  {
                     FileName = fileName,
                     MediaType = MediaType.Image
                  }
               };

               product.AddMedia(productMedia);
            }
         }
      }

      private static ProductPriceHistory CreatePriceHistory(
         User loginUser,
         Product product
      )
      {
         return new ProductPriceHistory
         {
            CreatedBy = loginUser,
            Product = product,
            Price = product.Price,
            OldPrice = product.OldPrice,
            SpecialPrice = product.SpecialPrice,
            SpecialPriceStart = product.SpecialPriceStart,
            SpecialPriceEnd = product.SpecialPriceEnd
         };
      }

      private static void MapProductLinkVmToProduct(
         ProductForm model,
         Product product
      )
      {
         foreach (var relatedProductVm in model.Product.RelatedProducts)
         {
            var productLink = new ProductLink
            {
               LinkType = ProductLinkType.Related,
               Product = product,
               LinkedProductId = relatedProductVm.Id
            };

            product.AddProductLinks(productLink);
         }
      }
   }
}
