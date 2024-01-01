using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Infrastructure.Models;

namespace MaziStore.Module.Shipments.Models
{
   public class ShipmentItem : EntityBase
   {
      public long ShipmentId { get; set; }

      public Shipment Shipment { get; set; }

      public long OrderItemId { get; set; }

      public long ProductId { get; set; }

      public Product Product { get; set; }

      public int Quantity { get; set; }
   }
}
