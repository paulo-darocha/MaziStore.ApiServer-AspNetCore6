using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Shipping.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.ShippingFree.Data
{
   public class ShippingFreeCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder
            .Entity<ShippingProvider>()
            .HasData(
               new ShippingProvider("FreeShip")
               {
                  Name = "Free Ship",
                  IsEnabled = true,
                  ConfigureUrl = "",
                  ShippingPriceServiceTypeName =
                     "MaziStore.Module.ShippingFree.Services."
                     + "FreeShippingServiceProvider,"
                     + "MaziStore.Module.ShippingFree",
                  AdditionalSettings = "{\"MinimumOrderAmount\" : 1}",
                  ToAllShippingEnabledCountries = true,
                  ToAllShippingEnabledStatesOrProvinces = true
               }
            );
      }
   }
}
