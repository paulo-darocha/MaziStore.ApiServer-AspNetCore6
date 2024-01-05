using MaziStore.Module.Infrastructure.Models;
using System;

namespace MaziStore.Module.ProductRecentlyViewed.Models
{
   public class RecentlyViewedProduct : EntityBase
   {
      public long UserId { get; set; }

      public long ProductId { get; set; }

      public DateTimeOffset LatestViewedOn { get; set; }
   }
}
