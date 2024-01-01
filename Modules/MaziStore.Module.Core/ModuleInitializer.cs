using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.Core
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<IMediaService, MediaService>();
         services.AddTransient<IWidgetInstanceService, WidgetInstanceService>();
      }
   }
}
