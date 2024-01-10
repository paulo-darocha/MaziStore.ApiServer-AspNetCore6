using MaziStore.Module.Core.Services;
using MaziStore.Module.Shipping.Models;
using MaziStore.Module.ShippingFree.Models;
using MaziStore.Module.ShippingPrices.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaziStore.Module.ShippingFree.Services
{
   public class FreeShippingServiceProvider : IShippingPriceServiceProvider
   {
      private readonly ICurrencyService _currencyService;

      public FreeShippingServiceProvider(ICurrencyService currencyService)
      {
         _currencyService = currencyService;
      }

      public Task<GetShippingPriceResponse> GetShippingPrices(
         GetShippingPriceRequest request,
         ShippingProvider provider
      )
      {
         var response = new GetShippingPriceResponse { IsSuccess = true };

         var freeShippingSetting = JsonSerializer.Deserialize<FreeShippingSetting>(
            provider.AdditionalSettings
         );

         if (request.OrderAmount < freeShippingSetting.MinimumOrderAmount)
         {
            return Task.FromResult(response);
         }

         response.ApplicablePrices.Add(
            new ShippingPrice(_currencyService) { Name = "Free", Price = 0 }
         );

         return Task.FromResult(response);
      }
   }
}
