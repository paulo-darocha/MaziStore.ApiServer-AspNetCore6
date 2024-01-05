using MaziStore.ApiServer.Home.Extensions.Identity;
using MaziStore.Module.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MaziStore.ApiServer.Home.Extensions
{
   public static class IdentityExtension
   {
      public static IServiceCollection AddMaziIdentity(
         this IServiceCollection services
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

         services.AddAuthentication();

         return services;
      }
   }
}
