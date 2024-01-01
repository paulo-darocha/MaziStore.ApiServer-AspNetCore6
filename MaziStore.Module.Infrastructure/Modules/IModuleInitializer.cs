using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.Infrastructure.Modules
{
   public interface IModuleInitializer
   {
      void ConfigureServices(IServiceCollection services);
   }
}
