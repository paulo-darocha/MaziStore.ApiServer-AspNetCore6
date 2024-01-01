using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace MaziStore.Module.ShoppingCart.Models
{
   public class Cart : EntityBase
   {
      public Cart()
      {
         CreatedOn = DateTimeOffset.Now;
         LatestUpdatedOn = DateTimeOffset.Now;
         IsActive = true;
      }

      public long CustomerId { get; set; }

      public User Customer { get; set; }

      public long CreatedById { get; set; }

      public User CreatedBy { get; set; }

      public DateTimeOffset CreatedOn { get; set; }

      public DateTimeOffset LatestUpdatedOn { get; set; }

      public bool IsActive { get; set; }

      public bool LockedOnCheckout { get; set; }

      [StringLength(450)]
      public string CouponCode { get; set; }

      [StringLength(450)]
      public string CouponRuleName { get; set; }

      [StringLength(450)]
      public string ShippingMethod { get; set; }

      public bool IsProductPriceIncludeTax { get; set; }

      public decimal? ShippingAmount { get; set; }

      public decimal? TaxAmount { get; set; }

      public IList<CartItem> Items { get; set; } = new List<CartItem>();

      // shipping data is type of Json
      public string ShippingData { get; set; }

      [StringLength(1000)]
      public string OrderNote { get; set; }
   }
}
