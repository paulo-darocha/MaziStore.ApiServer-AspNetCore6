using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Tax.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Tax.Data
{
   public class TaxCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder
            .Entity<TaxClass>()
            .HasData(new TaxClass(1) { Name = "Standard VAT" });

         modelBuilder
            .Entity<AppSetting>()
            .HasData(
               new AppSetting("Tax.DefaultTaxClassId")
               {
                  Module = "Tax",
                  IsVisibleInCommonSettingPage = true,
                  Value = "1"
               }
            );
      }
   }
}
