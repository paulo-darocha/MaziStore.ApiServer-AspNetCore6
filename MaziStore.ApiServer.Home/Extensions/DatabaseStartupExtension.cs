using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Data;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.ApiServer.Home.Extensions
{
   public static class DatabaseStartupExtension
   {
      public static void SetupMaziDatabase(this IApplicationBuilder app)
      {
         using (var scope = app.ApplicationServices.CreateScope())
         {
            MaziStoreDbContext dbContext =
               scope.ServiceProvider.GetRequiredService<MaziStoreDbContext>();

            //dbContext.Database.EnsureCreated();

            dbContext.Database.Migrate();
         }
      }
   }
}
