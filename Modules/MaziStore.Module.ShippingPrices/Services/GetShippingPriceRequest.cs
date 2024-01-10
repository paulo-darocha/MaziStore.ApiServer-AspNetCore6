using MaziStore.Module.Core.Models;
using System.Collections.Generic;

namespace MaziStore.Module.ShippingPrices.Services
{
   public class GetShippingPriceRequest
   {
      public Address ShippingAddress { get; set; }

      public Address WarehouseAddress { get; set; }

      public IList<ShippingItem> ShippingItem { get; set; } =
         new List<ShippingItem>();

      public decimal OrderAmount { get; set; }
   }
}
