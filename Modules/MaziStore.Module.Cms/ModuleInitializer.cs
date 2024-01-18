using MaziStore.Module.Cms.Events;
using MaziStore.Module.Core.Events;
using MaziStore.Module.Infrastructure.Modules;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.Cms
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<
            INotificationHandler<EntityDeleting>,
            EntityDeletingHandler
         >();
      }
   }
}
