using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Shipments.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Shipments.Data
{
   public class ShipmentCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<Shipment>(shippment =>
         {
            shippment
               .HasOne(x => x.CreatedBy)
               .WithMany()
               .HasForeignKey(x => x.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);
         });

         modelBuilder.Entity<Shipment>(shippment =>
         {
            shippment
               .HasOne(x => x.Order)
               .WithMany()
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Restrict);
         });

         modelBuilder.Entity<Shipment>(shippment =>
         {
            shippment
               .HasOne(x => x.Warehouse)
               .WithMany()
               .HasForeignKey(x => x.WarehouseId)
               .OnDelete(DeleteBehavior.Restrict);
         });
      }
   }
}
