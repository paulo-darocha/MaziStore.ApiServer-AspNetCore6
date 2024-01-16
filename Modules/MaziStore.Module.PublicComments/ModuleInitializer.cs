using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.PublicComments.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.PublicComments
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<IPublicCommentsService, PublicCommentsService>();
      }
   }
}
