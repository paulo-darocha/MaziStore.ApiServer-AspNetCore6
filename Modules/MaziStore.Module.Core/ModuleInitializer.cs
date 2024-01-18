using MaziStore.Module.Core.Events;
using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Modules;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.Core
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<IMediaService, MediaService>();
         services.AddTransient<IWidgetInstanceService, WidgetInstanceService>();
         services.AddScoped<ICurrencyService, CurrencyService>();
         services.AddScoped<IWorkContext, WorkContext>();
         services.AddTransient<
            INotificationHandler<UserSignedIn>,
            UserSignedInHandler
         >();
         services.AddTransient<IEntityService, EntityService>();
      }
   }
}
