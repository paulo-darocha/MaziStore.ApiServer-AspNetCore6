using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;
using MaziStore.Module.Orders.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using MaziStore.Module.Inventory.Models;

namespace MaziStore.Module.Shipments.Models
{
   public class Shipment : EntityBase
   {
      public Shipment()
      {
         CreatedOn = DateTimeOffset.Now;
         LatestUpdatedOn = DateTimeOffset.Now;
      }

      public long OrderId { get; set; }

      public Order Order { get; set; }

      [StringLength(450)]
      public string TrackingNumber { get; set; }

      public long WarehouseId { get; set; }

      public long? VendorId { get; set; }

      public Warehouse Warehouse { get; set; }

      public long CreatedById { get; set; }

      public User CreatedBy { get; set; }

      public DateTimeOffset CreatedOn { get; set; }

      public DateTimeOffset LatestUpdatedOn { get; set; }

      public IList<ShipmentItem> Items { get; set; } = new List<ShipmentItem>();
   }
}
