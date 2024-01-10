using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Shipping.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Shipping.Data
{
   public class ShippingCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder
            .Entity<ShippingProvider>()
            .ToTable("Shipping_ShippingProvider");
      }
   }
}
