using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Orders.Areas.Orders.ViewModels;
using MaziStore.Module.Orders.Services;
using MaziStore.Module.ShippingPrices.Services;
using MaziStore.Module.ShoppingCart.Models;
using MaziStore.Module.ShoppingCart.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaziStore.Module.Orders.Areas.Orders.Controllers
{
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   [ApiController]
   [Area("Orders")]
   [Route("api/[controller]")]
   public class CheckoutController : ControllerBase
   {
      private readonly IOrderService _orderService;
      private readonly IRepositoryWithTypedId<Country, string> _countryRepository;
      private readonly IRepository<StateOrProvince> _stateOrProvinceRepository;
      private readonly IRepository<UserAddress> _userAddressRepository;
      private readonly IShippingPriceService _shippingPriceService;
      private readonly ICartService _cartService;
      private readonly IRepository<Cart> _cartRepository;
      private readonly UserManager<User> _userManager;

      public CheckoutController(
         IOrderService orderService,
         IRepositoryWithTypedId<Country, string> countryRepository,
         IRepository<StateOrProvince> stateOrProvinceRepository,
         IRepository<UserAddress> userAddressRepository,
         IShippingPriceService shippingPriceService,
         ICartService cartService,
         IRepository<Cart> cartRepository,
         UserManager<User> userManager
      )
      {
         _orderService = orderService;
         _countryRepository = countryRepository;
         _stateOrProvinceRepository = stateOrProvinceRepository;
         _userAddressRepository = userAddressRepository;
         _shippingPriceService = shippingPriceService;
         _cartService = cartService;
         _cartRepository = cartRepository;
         _userManager = userManager;
      }

      [HttpGet("shipping")]
      public async Task<IActionResult> Shipping()
      {
         var currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);

         var cart = await _cartService.GetActiveCartDetails(currentUser.Id);
         if (cart == null || !cart.Items.Any())
         {
            return BadRequest($"Cart for user '{currentUser.Id}' not found.");
         }

         var model = new DeliveryInformationVm();

         PopulateShippingForm(model, currentUser);

         return Ok(model);
      }

      [HttpPost("update")]
      public async Task<IActionResult> UpdateTaxAndShippingPrices(
         [FromBody] TaxAndShippingPriceRequestVm model
      )
      {
         var currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);
         var cart = await _cartService.GetActiveCart(currentUser.Id);
         var orderTaxAndShippingPrice =
            await _orderService.UpdateTaxAndShippingPrices(cart.Id, model);

         return Ok(orderTaxAndShippingPrice);
      }

      [HttpPost("shipping")]
      public async Task<IActionResult> Shopping(DeliveryInformationVm model)
      {
         var currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);
         if (
            !model.NewAddressForm.IsValid() && model.ShippingAddressId == 0
            || model.NewBillingAddressForm != null
               && !model.NewBillingAddressForm.IsValid()
               && !model.UseShippingAddressAsBillingAddress
               && model.BillingAddressId == 0
         )
         {
            PopulateShippingForm(model, currentUser);
            return BadRequest(model);
         }

         var cart = await _cartService.GetActiveCart(currentUser.Id);
         if (cart == null)
         {
            throw new ApplicationException(
               $"Cart of user {currentUser.Id} cannot be found."
            );
         }

         cart.ShippingData = JsonSerializer.Serialize(model);
         await _cartRepository.SaveChangesRpAsync();
         return Ok(model);
      }

      // ///////////////////////////////////////////////

      private void PopulateShippingForm(
         DeliveryInformationVm model,
         User currentUser
      )
      {
         model.ExistingShippingAddresses = _userAddressRepository
            .QueryRp()
            .Where(
               x =>
                  x.AddressType == AddressType.Shipping
                  && x.UserId == currentUser.Id
            )
            .Select(
               x =>
                  new ShippingAddressVm
                  {
                     UserAddressId = x.Id,
                     ContactName = x.Address.ContactName,
                     Phone = x.Address.Phone,
                     AddressLine1 = x.Address.AddressLine1,
                     CityName = x.Address.City,
                     ZipCode = x.Address.ZipCode,
                     DistrictName = x.Address.District.Name,
                     StateOrProvinceName = x.Address.StateOrProvince.Name,
                     CountryName = x.Address.Country.Name,
                     IsCityEnabled = x.Address.Country.IsCityEnabled,
                     IsZipCodeEnabled = x.Address.Country.IsZipCodeEnabled,
                     IsDistrictEnabled = x.Address.Country.IsDistrictEnabled
                  }
            )
            .ToList();

         model.ShippingAddressId = currentUser.DefaultShippingAddressId ?? 0;
         model.UseShippingAddressAsBillingAddress = true;
         model.NewAddressForm.ShippableCountries = _countryRepository
            .QueryRp()
            .Where(x => x.IsShippingEnabled)
            .OrderBy(x => x.Name)
            .Select(
               x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }
            )
            .ToList();

         if (model.NewAddressForm.ShippableCountries.Count == 1)
         {
            var onlyShippableCountryId = model.NewAddressForm.ShippableCountries
               .First()
               .Value;

            model.NewAddressForm.StateOrProvinces = _stateOrProvinceRepository
               .QueryRp()
               .Where(x => x.CountryId == onlyShippableCountryId)
               .OrderBy(x => x.Name)
               .Select(
                  x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }
               )
               .ToList();
         }
      }
   }
}
