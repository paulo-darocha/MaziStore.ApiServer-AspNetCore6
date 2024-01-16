using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Orders.Areas.Orders.ViewModels;
using MaziStore.Module.Orders.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Orders.Areas.Orders.Controllers
{
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   [ApiController]
   [Area("Orders")]
   [Route("api/[controller]")]
   public class OrderController : ControllerBase
   {
      private readonly IMediaService _mediaService;
      private readonly IRepository<Order> _orderRepository;
      private readonly UserManager<User> _userManager;
      private readonly ICurrencyService _currencyService;

      public OrderController(
         IMediaService mediaService,
         IRepository<Order> orderRepository,
         UserManager<User> userManager,
         ICurrencyService currencyService
      )
      {
         _mediaService = mediaService;
         _orderRepository = orderRepository;
         _userManager = userManager;
         _currencyService = currencyService;
      }

      [HttpGet("list")]
      public async Task<IActionResult> OrderHistoryList()
      {
         var user = await _userManager.FindByEmailAsync(User.Identity.Name);
         var model = await _orderRepository
            .QueryRp()
            .Where(x => x.CustomerId == user.Id && x.ParentId == null)
            .Include(x => x.OrderItems)
            .ThenInclude(y => y.Product)
            .ThenInclude(z => z.ThumbnailImage)
            .Include(x => x.OrderItems)
            .ThenInclude(y => y.Product)
            .ThenInclude(z => z.OptionCombinations)
            .ThenInclude(w => w.Option)
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync();

         var model2 = model
            .Select(
               x =>
                  new OrderHistoryListItem(_currencyService)
                  {
                     Id = x.Id,
                     CreatedOn = x.CreatedOn,
                     SubTotal = x.SubTotal,
                     OrderStatus = x.OrderStatus,
                     OrderItems = x.OrderItems
                        .Select(
                           y =>
                              new OrderHistoryProductVm
                              {
                                 ProductId = y.ProductId,
                                 ProductName = y.Product.Name,
                                 Quantity = y.Quantity,
                                 ThumbnailImage = y.Product.ThumbnailImage.FileName,
                                 ProductOptions =
                                    y.Product.OptionCombinations.Select(
                                       z => z.Value
                                    )
                              }
                        )
                        .ToList()
                  }
            )
            .ToList();

         foreach (var item in model2)
         {
            foreach (var product in item.OrderItems)
            {
               product.ThumbnailImage = _mediaService.GetMediaUrl(
                  product.ThumbnailImage
               );
            }
         }

         return Ok(model2);
      }

      [HttpGet("{orderId}")]
      public async Task<IActionResult> OrderDetails(long orderId)
      {
         var user = await _userManager.FindByEmailAsync(User.Identity.Name);

         var order = _orderRepository
            .QueryRp()
            .Include(x => x.ShippingAddress)
            .ThenInclude(y => y.District)
            .Include(x => x.ShippingAddress)
            .ThenInclude(y => y.StateOrProvince)
            .Include(x => x.ShippingAddress)
            .ThenInclude(y => y.Country)
            .Include(x => x.OrderItems)
            .ThenInclude(y => y.Product)
            .ThenInclude(z => z.ThumbnailImage)
            .Include(x => x.OrderItems)
            .ThenInclude(y => y.Product)
            .ThenInclude(z => z.OptionCombinations)
            .ThenInclude(w => w.Option)
            .Include(x => x.Customer)
            .FirstOrDefault(x => x.Id == orderId);

         if (order == null)
         {
            return BadRequest($"Cannot find order id {orderId}");
         }

         if (order.CustomerId != user.Id)
         {
            return BadRequest($"User do not have permission to view this order.");
         }

         var model = new OrderDetailVm(_currencyService)
         {
            Id = order.Id,
            IsMasterOrder = order.IsMasterOrder,
            CreatedOn = order.CreatedOn,
            OrderStatus = (int)order.OrderStatus,
            OrderStatusString = order.OrderStatus.ToString(),
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.FullName,
            CustomerEmail = order.Customer.Email,
            ShippingMethod = order.ShippingMethod,
            PaymentMethod = order.PaymentMethod,
            PaymentFeeAmount = order.PaymentFeeAmount,
            Subtotal = order.SubTotal,
            DiscountAmount = order.DiscountAmount,
            SubTotalWithDiscount = order.SubTotalWithDiscount,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.ShippingFeeAmount,
            OrderTotal = order.OrderTotal,
            OrderNote = order.OrderNote,
            ShippingAddress = new ShippingAddressVm
            {
               AddressLine1 = order.ShippingAddress.AddressLine1,
               CityName = order.ShippingAddress.City,
               ZipCode = order.ShippingAddress.ZipCode,
               ContactName = order.ShippingAddress.ContactName,
               DistrictName = order.ShippingAddress.District?.Name,
               StateOrProvinceName = order.ShippingAddress.StateOrProvince.Name,
               Phone = order.ShippingAddress.Phone
            },
            OrderItems = order.OrderItems
               .Select(
                  x =>
                     new OrderItemVm(_currencyService)
                     {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductPrice = x.ProductPrice,
                        Quantity = x.Quantity,
                        DiscountAmount = x.DiscountAmount,
                        ProductImage = x.Product.ThumbnailImage.FileName,
                        TaxAmount = x.TaxAmount,
                        TaxPercent = x.TaxPercent,
                        VariationOptions = OrderItemVm.GetVariationOption(x.Product)
                     }
               )
               .ToList()
         };

         foreach (var item in model.OrderItems)
         {
            item.ProductImage = _mediaService.GetMediaUrl(item.ProductImage);
         }

         return Ok(model);
      }
   }
}
