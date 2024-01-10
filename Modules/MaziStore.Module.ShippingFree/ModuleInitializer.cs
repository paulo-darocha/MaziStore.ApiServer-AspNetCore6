using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.ShippingFree.Services;
using MaziStore.Module.ShippingPrices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.ShippingFree
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<
            IShippingPriceServiceProvider,
            FreeShippingServiceProvider
         >();
      }
   }
}
