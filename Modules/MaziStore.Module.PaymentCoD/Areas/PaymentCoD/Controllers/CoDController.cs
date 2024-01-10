using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Orders.Services;
using MaziStore.Module.PaymentCoD.Models;
using MaziStore.Module.Payments.Models;
using MaziStore.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using MaziStore.Module.ShoppingCart.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaziStore.Module.PaymentCoD.Areas.PaymentCoD.Controllers
{
   [ApiController]
   [Area("PaymentCoD")]
   [Route("api/[controller]")]
   public class CoDController : ControllerBase
   {
      private readonly IOrderService _orderService;
      private readonly IWorkContext _workContext;
      private readonly ICartService _cartService;
      private readonly IRepositoryWithTypedId<
         PaymentProvider,
         string
      > _paymentProviderRepository;
      private Lazy<CoDSetting> _setting;

      public CoDController(
         IOrderService orderService,
         IWorkContext workContext,
         ICartService cartService,
         IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository
      )
      {
         _orderService = orderService;
         _workContext = workContext;
         _cartService = cartService;
         _paymentProviderRepository = paymentProviderRepository;
         _setting = new Lazy<CoDSetting>(GetSetting());
      }

      [HttpGet]
      public async Task<IActionResult> CoDCheckout()
      {
         var currentUser = await _workContext.GetCurrentUser();
         var cart = await _cartService.GetActiveCartDetails(currentUser.Id);

         if (cart == null)
         {
            return BadRequest("Cart not found");
         }

         if (!ValidateCoD(cart))
         {
            return BadRequest("Payment Method is not eligible for this order.");
         }

         var calculatedFee = CalculateFee(cart);
         var orderCreatedResult = await _orderService.CreateOrder(
            cart.Id,
            "CashOnDelivery",
            calculatedFee
         );

         if (!orderCreatedResult.Success)
         {
            return BadRequest(orderCreatedResult.Error);
         }

         return Ok(orderCreatedResult.Value.Id);
      }

      // //////////////////////////////////////////////////////////

      private CoDSetting GetSetting()
      {
         var coDProvider = _paymentProviderRepository
            .QueryRp()
            .FirstOrDefault(x => x.Id == PaymentProviderHelper.CODProviderId);
         if (string.IsNullOrEmpty(coDProvider.AdditionalSettings))
         {
            return new CoDSetting();
         }

         var coDSetting = JsonSerializer.Deserialize<CoDSetting>(
            coDProvider.AdditionalSettings
         );

         return coDSetting;
      }

      private bool ValidateCoD(CartVm cart)
      {
         if (
            _setting.Value.MinOrderValue.HasValue
            && _setting.Value.MaxOrderValue.HasValue
         )
         {
            return false;
         }

         if (
            _setting.Value.MaxOrderValue.HasValue
            && _setting.Value.MaxOrderValue.Value < cart.OrderTotal
         )
         {
            return false;
         }

         return true;
      }

      private decimal CalculateFee(CartVm cart)
      {
         var percent = _setting.Value.PaymentFee;
         return (cart.OrderTotal / 100) * percent;
      }
   }
}
