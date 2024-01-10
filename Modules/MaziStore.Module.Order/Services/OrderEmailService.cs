using MaziStore.Module.Core.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Orders.Models;
using System.Threading.Tasks;

namespace MaziStore.Module.Orders.Services
{
   public class OrderEmailService : IOrderEmailService
   {
      private readonly IEmailSender _emailSender;

      public OrderEmailService(IEmailSender emailSender)
      {
         _emailSender = emailSender;
      }

      public async Task SendEmailToUser(User user, Order order)
      {
         var emailBody = "";
         var emailSubject = $"Order Information #{order.Id}";
         await _emailSender.SendEmailAsync(
            user.Email,
            emailSubject,
            emailBody,
            true
         );
      }
   }
}
