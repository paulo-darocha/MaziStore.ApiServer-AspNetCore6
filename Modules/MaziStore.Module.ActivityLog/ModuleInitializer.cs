using MaziStore.Module.ActivityLog.Events;
using MaziStore.Module.Core.Events;
using MaziStore.Module.Infrastructure.Modules;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.ActivityLog
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<
            INotificationHandler<EntityViewed>,
            EntityViewedHandler
         >();
      }
   }
}
