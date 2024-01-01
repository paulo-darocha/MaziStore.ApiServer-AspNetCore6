using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;
using System;

namespace MaziStore.Module.Pricing.Models
{
   public class CartRuleUsage : EntityBase
   {
      public CartRuleUsage()
      {
         CreatedOn = DateTimeOffset.Now;
      }

      public long CartRuleId { get; set; }

      public CartRule CartRule { get; set; }

      public long? CouponId { get; set; }

      public Coupon Coupon { get; set; }

      public long UserId { get; set; }

      public User User { get; set; }

      public long OrderId { get; set; }

      public DateTimeOffset CreatedOn { get; set; }
   }
}
