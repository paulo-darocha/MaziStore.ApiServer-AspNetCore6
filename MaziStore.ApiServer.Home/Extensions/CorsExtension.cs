using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.ApiServer.Home.Extensions
{
   public static class CorsExtension
   {
      public static IServiceCollection AddMaziCors(this IServiceCollection services)
      {
         services.AddCors(
            options =>
               options.AddPolicy(
                  "CorsPolicy",
                  builder =>
                     builder
                        .WithOrigins(
                           "http://localhost:8000",
                           "http://3.10.173.253:8000",
                           "http://store.paulodarocha.eu",
                           "https://store.paulodarocha.eu"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
               )
         );

         return services;
      }
   }
}
