using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.Pricing.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.Pricing
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<ICouponService, CouponService>();
      }
   }
}
