using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Orders.Models;
using MediatR;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MaziStore.Module.Orders.Events
{
   public class OrderCreatedCreateOrderHistoryHandler
      : INotificationHandler<OrderCreated>
   {
      private readonly IRepository<OrderHistory> _orderHistoryRepository;

      public OrderCreatedCreateOrderHistoryHandler(
         IRepository<OrderHistory> orderHistoryRepository
      )
      {
         _orderHistoryRepository = orderHistoryRepository;
      }

      public async Task Handle(
         OrderCreated notification,
         CancellationToken cancellationToken
      )
      {
         var orderHistory = new OrderHistory
         {
            OrderId = notification.Order.Id,
            CreatedOn = DateTimeOffset.Now,
            CreatedById = notification.Order.CreatedById,
            NewStatus = OrderStatus.New,
            Note = notification.Order.OrderNote
         };

         if (notification.Order != null)
         {
            orderHistory.OrderSnapshot = JsonSerializer.Serialize(
               notification.Order
            );
         }

         _orderHistoryRepository.AddRp(orderHistory);
         await _orderHistoryRepository.SaveChangesRpAsync();
      }
   }
}
