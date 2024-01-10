using MaziStore.Module.Orders.Models;
using MediatR;

namespace MaziStore.Module.Orders.Events
{
   public class AfterOrderCreated : INotification
   {
      public AfterOrderCreated(Order order)
      {
         Order = order;
      }

      public Order Order { get; }
   }
}
