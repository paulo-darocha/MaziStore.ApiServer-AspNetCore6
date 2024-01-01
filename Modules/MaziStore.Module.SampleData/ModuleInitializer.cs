using MaziStore.Module.Infrastructure.Modules;
using MaziStore.Module.SampleData.Data;
using MaziStore.Module.SampleData.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.SampleData
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddTransient<ISqlRepository, SqlRepository>();
         services.AddTransient<ISampleDataService, SampleDataService>();
      }
   }
}
