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
                        .WithOrigins("https://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
               )
         );

         return services;
      }
   }
}
