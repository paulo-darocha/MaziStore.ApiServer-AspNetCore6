using MaziStore.Module.Core.Events;
using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.ProductRecentlyViewed.Data;
using MaziStore.Module.ProductRecentlyViewed.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.ProductRecentlyViewed
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<
            IRecentlyViewedProductRepository,
            RecentlyViewedProductRepository
         >();
         services.AddTransient<
            INotificationHandler<EntityViewed>,
            EntityViewedHandler
         >();
      }
   }
}
