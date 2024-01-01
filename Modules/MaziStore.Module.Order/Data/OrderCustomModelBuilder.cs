using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Orders.Data
{
   public class OrderCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<OrderAddress>(orderAddress =>
         {
            orderAddress
               .HasOne(x => x.District)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            orderAddress
               .HasOne(x => x.StateOrProvince)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            orderAddress
               .HasOne(x => x.Country)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);
         });

         modelBuilder.Entity<Order>(order =>
         {
            order
               .HasOne(x => x.ShippingAddress)
               .WithMany()
               .HasForeignKey(x => x.ShippingAddressId)
               .OnDelete(DeleteBehavior.Restrict);
         });

         modelBuilder.Entity<Order>(order =>
         {
            order
               .HasOne(x => x.BillingAddress)
               .WithMany()
               .HasForeignKey(x => x.BillingAddressId)
               .OnDelete(DeleteBehavior.Restrict);
         });

         modelBuilder.Entity<OrderHistory>(orderHistory =>
         {
            orderHistory
               .HasOne(x => x.CreatedBy)
               .WithMany()
               .HasForeignKey(x => x.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);
         });
      }
   }
}
