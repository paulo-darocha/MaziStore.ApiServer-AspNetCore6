using MaziStore.Module.Core.Models;
using MaziStore.Module.Orders.Models;
using System.Threading.Tasks;

namespace MaziStore.Module.Orders.Services
{
   public interface IOrderEmailService
   {
      Task SendEmailToUser(User user, Order order);
   }
}
