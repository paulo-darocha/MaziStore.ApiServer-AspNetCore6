﻿using System.Collections.Generic;

namespace MaziStore.Module.Pricing.Services
{
   public class CartInfoForCoupon
   {
      public IList<CartItemForCoupon> Items { get; set; } =
         new List<CartItemForCoupon>();
   }
}
