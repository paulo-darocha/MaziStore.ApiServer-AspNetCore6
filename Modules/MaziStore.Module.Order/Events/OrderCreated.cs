using MaziStore.Module.Orders.Models;
using MediatR;

namespace MaziStore.Module.Orders.Events
{
   public class OrderCreated : INotification
   {
      public OrderCreated(Order order)
      {
         Order = order;
      }

      public Order Order { get; }
   }
}
