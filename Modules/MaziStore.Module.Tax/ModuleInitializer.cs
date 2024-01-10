using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.Tax.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.Tax
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<ITaxService, TaxService>();
      }
   }
}
