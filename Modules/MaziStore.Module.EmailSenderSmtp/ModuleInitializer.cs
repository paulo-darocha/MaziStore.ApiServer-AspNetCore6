using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.Module.EmailSenderSmtp
{
   public class ModuleInitializer : IModuleInitializer
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddScoped<IEmailSender, EmailSender>();
      }
   }
}
