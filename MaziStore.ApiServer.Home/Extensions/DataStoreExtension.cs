using MaziStore.Module.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MaziStore.ApiServer.Home.Extensions
{
   public static class DataStoreExtension
   {
      public static IServiceCollection AddMaziDataStore(
         this IServiceCollection services,
         IConfiguration configuration
      )
      {
         services.AddDbContext<MaziStoreDbContext>(options =>
         {
            var connectionString = configuration.GetConnectionString(
               "MaziStoreConnection"
            );
            Console.WriteLine($"\n\nConnection String: {connectionString}\n");
            options.UseSqlServer(
               connectionString,
               opt => opt.MigrationsAssembly("MaziStore.ApiServer.Home")
            );
            options.EnableSensitiveDataLogging(true);
         });

         return services;
      }
   }
}
