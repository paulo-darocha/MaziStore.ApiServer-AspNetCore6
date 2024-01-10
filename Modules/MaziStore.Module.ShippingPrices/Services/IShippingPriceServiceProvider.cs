using MaziStore.Module.Shipping.Models;
using System.Threading.Tasks;

namespace MaziStore.Module.ShippingPrices.Services
{
   public interface IShippingPriceServiceProvider
   {
      Task<GetShippingPriceResponse> GetShippingPrices(
         GetShippingPriceRequest request,
         ShippingProvider provider
      );
   }
}
