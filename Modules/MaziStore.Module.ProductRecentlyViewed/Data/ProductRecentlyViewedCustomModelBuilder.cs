using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace MaziStore.Module.ProductRecentlyViewed.Data
{
   public class ProductRecentlyViewedCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder
            .Entity<Widget>()
            .HasData(
               new Widget("RecentlyViewedWidget")
               {
                  Name = "Recently Viewed Widget",
                  ViewComponentName = "RecentlyViewedWidget",
                  CreateUrl = "widget-recently-viewed-create",
                  EditUrl = "widget-recently-viewed-edit",
                  CreatedOn = new DateTimeOffset(
                     new DateTime(
                        2018,
                        5,
                        29,
                        4,
                        33,
                        39,
                        164,
                        DateTimeKind.Unspecified
                     ),
                     new TimeSpan(0, 7, 0, 0, 0)
                  )
               }
            );
      }
   }
}
