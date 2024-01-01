using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System;

namespace MaziStore.Module.Orders.Models
{
   public class OrderHistory : EntityBase
   {
      public long OrderId { get; set; }

      public Order Order { get; set; }

      public OrderStatus? OldStatus { get; set; }

      public OrderStatus NewStatus { get; set; }

      public string OrderSnapshot { get; set; }

      [StringLength(1000)]
      public string Note { get; set; }

      public DateTimeOffset CreatedOn { get; set; }

      public long CreatedById { get; set; }

      public User CreatedBy { get; set; }
   }
}
