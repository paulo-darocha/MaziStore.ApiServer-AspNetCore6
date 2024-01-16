using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Orders.Services;
using MaziStore.Module.Payments.Areas.Payments.ViewModels;
using MaziStore.Module.Payments.Models;
using MaziStore.Module.ShoppingCart.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Payments.Areas.Payments.Controllers
{
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
      private readonly UserManager<User> _userManager;

      public CheckoutController(
         IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
         ICartService cartService,
         IOrderService orderService,
         UserManager<User> userManager
      )
      {
         _paymentProviderRepository = paymentProviderRepository;
         _cartService = cartService;
         _orderService = orderService;
         _userManager = userManager;
      }

      [HttpGet("payment")]
      public async Task<IActionResult> Payment()
      {
         var currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);
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
