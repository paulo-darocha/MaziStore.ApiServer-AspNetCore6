using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Inventory.Data
{
   public class InventoryCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder
            .Entity<Warehouse>()
            .HasData(
               new Warehouse(1) { Name = "Default warehouse", AddressId = 1 }
            );
      }
   }
}
