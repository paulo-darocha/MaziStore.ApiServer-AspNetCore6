using MaziStore.Module.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.ApiServer.Home.Extensions
{
   public static class DataStoreExtension
   {
      public static IServiceCollection AddCustomizedDataStore(
         this IServiceCollection services,
         IConfiguration configuration
      )
      {
         services.AddDbContext<MaziStoreDbContext>(options =>
         {
            options.UseSqlServer(
               configuration.GetConnectionString("DefaultConnection"),
               opt => opt.MigrationsAssembly("MaziStore.ApiServer.Home")
            );
            options.EnableSensitiveDataLogging(true);
         });

         return services;
      }
   }
}
