using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Orders.Services;
using MaziStore.Module.Payments.Areas.Payments.ViewModels;
using MaziStore.Module.Payments.Models;
using MaziStore.Module.ShoppingCart.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Payments.Areas.Payments.Controllers
{
   [ApiController]
   [Area("Payments")]
   [Route("api/[controller]")]
   public class CheckoutController : ControllerBase
   {
      private readonly IRepositoryWithTypedId<
         PaymentProvider,
         string
      > _paymentProviderRepository;
      private readonly ICartService _cartService;
      private readonly IOrderService _orderService;
      private readonly IWorkContext _workContext;

      public CheckoutController(
         IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
         ICartService cartService,
         IOrderService orderService,
         IWorkContext workContext
      )
      {
         _paymentProviderRepository = paymentProviderRepository;
         _cartService = cartService;
         _orderService = orderService;
         _workContext = workContext;
      }

      [HttpGet("payment")]
      public async Task<IActionResult> Payment()
      {
         var currentUser = await _workContext.GetCurrentUser();
         var cart = await _cartService.GetActiveCart(currentUser.Id);
         if (cart == null)
         {
            return NotFound("No cart found for current user.");
         }

         cart.LockedOnCheckout = true;
         await _paymentProviderRepository.SaveChangesRpAsync();

         var checkoutPaymentForm = new CheckoutPaymentForm();
         checkoutPaymentForm.PaymentProviders = await _paymentProviderRepository
            .QueryRp()
            .Where(x => x.IsEnabled)
            .Select(
               x =>
                  new PaymentProviderVm
                  {
                     Id = x.Id,
                     Name = x.Name,
                     LandingViewComponentName = x.LandingViewComponentName
                  }
            )
            .ToListAsync();

         return Ok(checkoutPaymentForm);
      }
   }
}
