using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.ApiServer.Home.Extensions
{
   public static class CorsExtension
   {
      public static IServiceCollection AddMaziCors(
         this IServiceCollection services,
         IConfiguration configuration
      )
      {
         services.AddCors(
            options =>
               options.AddPolicy(
                  "CorsPolicy",
                  builder =>
                     builder
                        .WithOrigins(configuration["AllowedOrigin"])
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
               )
         );

         return services;
      }
   }
}
