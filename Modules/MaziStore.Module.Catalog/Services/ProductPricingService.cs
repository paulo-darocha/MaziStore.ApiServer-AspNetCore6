using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Services;
using System;

namespace MaziStore.Module.Catalog.Services
{
   public class ProductPricingService : IProductPricingService
   {
      private ICurrencyService _currencyService;

      public ProductPricingService(ICurrencyService currencyService)
      {
         _currencyService = currencyService;
      }

      public CalculatedProductPrice CalculateProductPrice(
         ProductThumbnail productThumbnail
      )
      {
         return CalculateProductPrice(
            productThumbnail.Price,
            productThumbnail.OldPrice,
            productThumbnail.SpecialPrice,
            productThumbnail.SpecialPriceStart,
            productThumbnail.SpecialPriceEnd
         );
      }

      public CalculatedProductPrice CalculateProductPrice(Product product)
      {
         return CalculateProductPrice(
            product.Price,
            product.OldPrice,
            product.SpecialPrice,
            product.SpecialPriceStart,
            product.SpecialPriceEnd
         );
      }

      public CalculatedProductPrice CalculateProductPrice(
         decimal price,
         decimal? oldPrice,
         decimal? specialPrice,
         DateTimeOffset? specialPriceStart,
         DateTimeOffset? specialPriceEnd
      )
      {
         var percentOfSaving = 0;
         var calculatedProductPrice = price;

         if (
            specialPrice.HasValue
            && specialPriceStart < DateTimeOffset.Now
            && DateTimeOffset.Now < specialPriceEnd
         )
         {
            calculatedProductPrice = specialPrice.Value;
            if (!oldPrice.HasValue || oldPrice < price)
            {
               oldPrice = price;
            }
         }

         if (
            oldPrice.HasValue
            && oldPrice.Value > 0
            && oldPrice > calculatedProductPrice
         )
         {
            percentOfSaving = (int)(
               100 - Math.Ceiling((calculatedProductPrice / oldPrice.Value) * 100)
            );
         }

         return new CalculatedProductPrice(_currencyService)
         {
            Price = calculatedProductPrice,
            OldPrice = oldPrice,
            PercentOfSaving = percentOfSaving
         };
      }
   }
}
