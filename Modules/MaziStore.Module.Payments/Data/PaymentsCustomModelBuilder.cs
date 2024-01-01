using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Payments.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Payments.Data
{
   public class PaymentsCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<PaymentProvider>().ToTable("Payments_PaymentProvider");
      }
   }
}
