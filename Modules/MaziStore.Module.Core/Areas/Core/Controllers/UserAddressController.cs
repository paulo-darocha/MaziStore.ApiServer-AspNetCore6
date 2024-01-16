using MaziStore.Module.Core.Areas.Core.ViewModels;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Areas.Core.Controllers
{
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   [ApiController]
   [Area("Core")]
   [Route("api/[controller]")]
   public class UserAddressController : ControllerBase
   {
      private readonly IRepository<UserAddress> _userAddressRepository;
      private readonly IRepositoryWithTypedId<Country, string> _countryRepository;
      private readonly IRepository<StateOrProvince> _stateOrProvinceRepository;
      private readonly IRepository<District> _districtRepository;
      private readonly IRepository<User> _userRepository;
      private readonly UserManager<User> _userManager;

      public UserAddressController(
         IRepository<UserAddress> userAddressRepository,
         IRepositoryWithTypedId<Country, string> countryRepository,
         IRepository<StateOrProvince> stateOrProvinceRepository,
         IRepository<District> districtRepository,
         IRepository<User> userRepository,
         UserManager<User> userManager
      )
      {
         _userAddressRepository = userAddressRepository;
         _countryRepository = countryRepository;
         _stateOrProvinceRepository = stateOrProvinceRepository;
         _districtRepository = districtRepository;
         _userRepository = userRepository;
         _userManager = userManager;
      }

      [HttpGet]
      public async Task<IActionResult> List()
      {
         var currentUser = await _userManager.FindByEmailAsync(User.Identity.Name);
         var model = _userAddressRepository
            .QueryRp()
            .Where(
               x =>
                  x.AddressType == AddressType.Shipping
                  && x.UserId == currentUser.Id
            )
            .Select(
               x =>
                  new UserAddressListItem
                  {
                     AddressId = x.AddressId,
                     UserAddressId = x.Id,
                     ContactName = x.Address.ContactName,
                     Phone = x.Address.Phone,
                     AddressLine1 = x.Address.AddressLine1,
                     AddressLine2 = x.Address.AddressLine2,
                     DistrictName = x.Address.District.Name,
                     StateOrProvinceName = x.Address.StateOrProvince.Name,
                     CountryName = x.Address.Country.Name,
                     DisplayCity = x.Address.Country.IsCityEnabled,
                     DisplayZipCode = x.Address.Country.IsZipCodeEnabled,
                     DisplayDistrict = x.Address.Country.IsDistrictEnabled,
                     CityName = x.Address.City
                  }
            )
            .ToList();

         foreach (var item in model)
         {
            item.IsDefaultShippingAddress =
               item.UserAddressId == currentUser.DefaultShippingAddressId;
         }

         return Ok(model);
      }

      [HttpGet("{countryId}")]
      public async Task<IActionResult> GetStateFromCountry(string countryId)
      {
         var country = await _countryRepository
            .QueryRp()
            .Include(x => x.StatesOrProvinces)
            .FirstOrDefaultAsync(x => x.Id == countryId);

         if (country == null)
         {
            return NotFound($"Country - id {countryId} - not found.");
         }

         var model = new
         {
            CountryId = country.Id,
            CountryName = country.Name,
            country.IsBillingEnabled,
            country.IsShippingEnabled,
            country.IsCityEnabled,
            country.IsZipCodeEnabled,
            country.IsDistrictEnabled,
            StatesOrProvinces = country.StatesOrProvinces.Select(
               x => new { x.Id, x.Name }
            )
         };

         return Ok(model);
      }

      [HttpGet("create")]
      public IActionResult Create()
      {
         var model = new AddressFormVm();

         PopulateAddressFormData(model);

         return Ok(model);
      }

      [HttpPost("create")]
      public async Task<IActionResult> Create(AddressFormVm model)
      {
         if (ModelState.IsValid)
         {
            var currentUser = await _userManager.FindByEmailAsync(
               User.Identity.Name
            );
            var address = new Address
            {
               AddressLine1 = model.AddressLine1,
               AddressLine2 = model.AddressLine2,
               ContactName = model.ContactName,
               CountryId = model.CountryId,
               StateOrProvinceId = model.StateOrProvinceId,
               DistrictId = model.DistrictId,
               City = model.City,
               ZipCode = model.ZipCode,
               Phone = model.Phone
            };

            var userAddress = new UserAddress
            {
               Address = address,
               AddressType = AddressType.Shipping,
               UserId = currentUser.Id
            };

            _userAddressRepository.AddRp(userAddress);
            _userAddressRepository.SaveChangesRp();
            return await List();
         }

         PopulateAddressFormData(model);
         return BadRequest(model);
      }

      //[HttpGet("addresses")]
      //public async Task<IActionResult> List()
      //{
      //   var currentUser = await _workContext.GetCurrentUser();
      //   var model = _userAddressRepository
      //      .QueryRp()
      //      .Where(
      //         x =>
      //            x.AddressType == AddressType.Shipping
      //            && x.UserId == currentUser.Id
      //      )
      //      .Select(
      //         x =>
      //            new UserAddressListItem
      //            {
      //               AddressId = x.AddressId,
      //               UserAddressId = x.Id,
      //               ContactName = x.Address.ContactName,
      //               Phone = x.Address.Phone,
      //               AddressLine1 = x.Address.AddressLine1,
      //               AddressLine2 = x.Address.AddressLine2,
      //               DistrictName = x.Address.District.Name,
      //               StateOrProvinceName = x.Address.StateOrProvince.Name,
      //               CountryName = x.Address.Country.Name,
      //               DisplayCity = x.Address.Country.IsCityEnabled,
      //               DisplayZipCode = x.Address.Country.IsCityEnabled,
      //               DisplayDistrict = x.Address.Country.IsDistrictEnabled
      //            }
      //      )
      //      .ToList();

      //   foreach (var item in model)
      //   {
      //      item.IsDefaultShippingAddress =
      //         item.UserAddressId == currentUser.DefaultShippingAddressId;
      //   }

      //   return Ok(model);
      //}

      // ////////////////////////////////////////////////
      private void PopulateAddressFormData(AddressFormVm model)
      {
         var shippableCountries = _countryRepository
            .QueryRp()
            .Where(x => x.IsShippingEnabled)
            .OrderBy(x => x.Name);

         if (!shippableCountries.Any())
         {
            return;
         }

         model.ShippableCountries = shippableCountries
            .Select(
               x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }
            )
            .ToList();

         var selectedShippableCountryId = !string.IsNullOrEmpty(model.CountryId)
            ? model.CountryId
            : model.ShippableCountries.First().Value;

         var selectedCountry = shippableCountries.FirstOrDefault(
            x => x.Id == selectedShippableCountryId
         );

         model.StateOrProvinces = _stateOrProvinceRepository
            .QueryRp()
            .Where(x => x.CountryId == selectedShippableCountryId)
            .OrderBy(x => x.Name)
            .Select(
               x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }
            )
            .ToList();

         if (model.StateOrProvinceId > 0)
         {
            model.Districts = _districtRepository
               .QueryRp()
               .Where(x => x.StateOrProvinceId == model.StateOrProvinceId)
               .OrderBy(x => x.Name)
               .Select(
                  x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }
               )
               .ToList();
         }
      }
   }
}
