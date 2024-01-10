using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.ShippingPrices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.ShippingPrices
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<IShippingPriceService, ShippingPriceService>();
      }
   }
}
