using MaziStore.ApiServer.Home.Extensions.Identity;
using MaziStore.Module.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MaziStore.ApiServer.Home.Extensions
{
   public static class IdentityExtension
   {
      public static IServiceCollection AddMaziIdentity(
         this IServiceCollection services,
         IConfiguration configuration
      )
      {
         services.ConfigureApplicationCookie(
            options => options.Events.DisableRedirectionForApiClients()
         );

         services
            .AddIdentity<User, Role>()
            .AddRoleStore<MaziRoleStore>()
            .AddUserStore<MaziUserStore>()
            .AddSignInManager<MaziSignInManager<User>>()
            .AddDefaultTokenProviders();

         services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(
               JwtBearerDefaults.AuthenticationScheme,
               options =>
               {
                  options.TokenValidationParameters.ValidateAudience = false;
                  options.TokenValidationParameters.ValidateIssuer = false;
                  options.TokenValidationParameters.IssuerSigningKey =
                     new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["BearerTokens:Key"])
                     );
               }
            );

         return services;
      }
   }
}
