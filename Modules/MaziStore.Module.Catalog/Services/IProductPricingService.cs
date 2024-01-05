using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using System;

namespace MaziStore.Module.Catalog.Services
{
   public interface IProductPricingService
   {
      CalculatedProductPrice CalculateProductPrice(
         ProductThumbnail productThumbnail
      );

      CalculatedProductPrice CalculateProductPrice(Product product);

      CalculatedProductPrice CalculateProductPrice(
         decimal price,
         decimal? oldPrice,
         decimal? specialPrice,
         DateTimeOffset? specialPriceStart,
         DateTimeOffset? specialPriceEnd
      );
   }
}
