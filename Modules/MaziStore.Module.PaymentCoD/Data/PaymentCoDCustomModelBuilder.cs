using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Payments.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.PaymentCoD.Data
{
   public class PaymentCoDCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder
            .Entity<PaymentProvider>()
            .HasData(
               new PaymentProvider("CoD")
               {
                  Name = "Cash On Delivery",
                  LandingViewComponentName = "CoDLanding",
                  ConfigureUrl = "payments-cod-config",
                  IsEnabled = true,
                  AdditionalSettings = null
               }
            );
      }
   }
}
