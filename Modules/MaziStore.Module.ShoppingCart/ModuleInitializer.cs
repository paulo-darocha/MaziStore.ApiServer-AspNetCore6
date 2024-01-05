using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.ShoppingCart.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.ShoppingCart
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<ICartService, CartService>();
      }
   }
}
