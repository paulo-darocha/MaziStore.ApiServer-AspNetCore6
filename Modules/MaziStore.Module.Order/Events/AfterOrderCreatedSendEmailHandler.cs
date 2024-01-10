using MaziStore.Module.Orders.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace MaziStore.Module.Orders.Events
{
   public class AfterOrderCreatedSendEmailHandler
      : INotificationHandler<AfterOrderCreated>
   {
      private readonly IOrderEmailService _orderEmailService;

      public AfterOrderCreatedSendEmailHandler(IOrderEmailService orderEmailService)
      {
         _orderEmailService = orderEmailService;
      }

      public async Task Handle(
         AfterOrderCreated notification,
         CancellationToken cancellationToken
      )
      {
         await _orderEmailService.SendEmailToUser(
            notification.Order.Customer,
            notification.Order
         );
      }
   }
}
