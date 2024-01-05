using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Pricing.Services;
using MaziStore.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using MaziStore.Module.ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.ShoppingCart.Services
{
   public class CartService : ICartService
   {
      private readonly IRepository<Cart> _cartRepository;
      private readonly IRepository<CartItem> _cartItemRepository;
      private readonly IMediaService _mediaService;
      private readonly ICouponService _couponService;
      private readonly bool _isProductPriceIncludeTax;
      private readonly ICurrencyService _currencyService;

      public CartService(
         IRepository<Cart> cartRepository,
         IRepository<CartItem> cartItemRepository,
         IMediaService mediaService,
         ICouponService couponService,
         ICurrencyService currencyService
      )
      {
         _cartRepository = cartRepository;
         _cartItemRepository = cartItemRepository;
         _mediaService = mediaService;
         _couponService = couponService;
         _isProductPriceIncludeTax = true;
         _currencyService = currencyService;
      }

      public IQueryable<Cart> Query()
      {
         return _cartRepository.QueryRp();
      }

      public Task<Cart> GetActiveCart(long customerId)
      {
         return GetActiveCart(customerId, customerId);
      }

      public async Task<Cart> GetActiveCart(long customerId, long createdById)
      {
         return await _cartRepository
            .QueryRp()
            .Include(x => x.Items)
            .Where(
               x =>
                  x.CustomerId == customerId
                  && x.CreatedById == createdById
                  && x.IsActive
            )
            .FirstOrDefaultAsync();
      }

      public async Task<AddToCartResult> AddToCart(
         long customerId,
         long productId,
         int quantity
      )
      {
         return await AddToCart(customerId, customerId, productId, quantity);
      }

      public async Task<AddToCartResult> AddToCart(
         long customerId,
         long createdById,
         long productId,
         int quantity
      )
      {
         var addToCartResult = new AddToCartResult { Success = false };

         if (quantity <= 0)
         {
            addToCartResult.ErrorMessage = "The quantity must be larger then zero.";
            addToCartResult.ErrorCode = "wrong-quantity";
            return addToCartResult;
         }

         var cart = await GetActiveCart(customerId, createdById);
         if (cart == null)
         {
            cart = new Cart
            {
               CustomerId = customerId,
               CreatedById = createdById,
               IsProductPriceIncludeTax = _isProductPriceIncludeTax
            };
            _cartRepository.AddRp(cart);
         }
         else
         {
            if (cart.LockedOnCheckout)
            {
               addToCartResult.ErrorMessage =
                  "The cart is locked for checkout. Please complete the checkout first.";
               addToCartResult.ErrorCode = "cart-locked";
               return addToCartResult;
            }

            cart = await _cartRepository
               .QueryRp()
               .Include(x => x.Items)
               .FirstOrDefaultAsync(x => x.Id == cart.Id);
         }

         var cartItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
         if (cartItem == null)
         {
            cartItem = new CartItem
            {
               Cart = cart,
               ProductId = productId,
               Quantity = quantity,
               CreatedOn = DateTimeOffset.Now
            };
            cart.Items.Add(cartItem);
         }
         else
         {
            cartItem.Quantity = cartItem.Quantity + quantity;
         }

         await _cartRepository.SaveChangesRpAsync();

         addToCartResult.Success = true;
         return addToCartResult;
      }

      public async Task<CartVm> GetActiveCartDetails(long customerId)
      {
         return await GetActiveCartDetails(customerId, customerId);
      }

      public async Task<CartVm> GetActiveCartDetails(
         long customerId,
         long createdById
      )
      {
         var cart = await GetActiveCart(customerId, createdById);
         if (cart == null)
         {
            return null;
         }

         var cartVm = new CartVm(_currencyService)
         {
            Id = cart.Id,
            CouponCode = cart.CouponCode,
            IsProductPriceIncludeTax = cart.IsProductPriceIncludeTax,
            TaxAmount = cart.TaxAmount,
            ShippingAmount = cart.ShippingAmount,
            OrderNote = cart.OrderNote,
            LockedOnCheckout = cart.LockedOnCheckout
         };

         cartVm.Items = _cartItemRepository
            .QueryRp()
            .Include(x => x.Product)
            .ThenInclude(y => y.ThumbnailImage)
            .Include(x => x.Product)
            .ThenInclude(y => y.OptionCombinations)
            .ThenInclude(z => z.Option)
            .Where(x => x.CartId == cart.Id)
            .ToList()
            .Select(
               x =>
                  new CartItemVm(_currencyService)
                  {
                     Id = x.Id,
                     ProductId = x.ProductId,
                     ProductName = x.Product.Name,
                     ProductPrice = x.Product.Price,
                     ProductStockQuantity = x.Product.StockQuantity,
                     ProductStockTrackingIsEnabled =
                        x.Product.StockTrackingIsEnabled,
                     IsProductAvailabeToOrder =
                        x.Product.IsAllowToOrder
                        && x.Product.IsPublished
                        && !x.Product.IsDeleted,
                     ProductImage = _mediaService.GetThumbnailUrl(
                        x.Product.ThumbnailImage
                     ),
                     Quantity = x.Quantity,
                     VariationOptions = CartItemVm.GetVariationOption(x.Product)
                  }
            )
            .ToList();

         cartVm.SubTotal = cartVm.Items.Sum(x => x.Quantity * x.ProductPrice);
         if (!string.IsNullOrWhiteSpace(cartVm.CouponCode))
         {
            var cartInfoForCoupon = new CartInfoForCoupon
            {
               Items = cartVm.Items
                  .Select(
                     y =>
                        new CartItemForCoupon
                        {
                           ProductId = y.ProductId,
                           Quantity = y.Quantity
                        }
                  )
                  .ToList()
            };
            var couponValidationRestul = await _couponService.Validate(
               customerId,
               cartVm.CouponCode,
               cartInfoForCoupon
            );
            if (couponValidationRestul.Succeeded)
            {
               cartVm.Discount = couponValidationRestul.DiscountAmount;
            }
            else
            {
               cartVm.CouponValidationErrorMessage =
                  couponValidationRestul.ErrorMessage;
            }
         }

         return cartVm;
      }

      public async Task<CouponValidationResult> ApplyCoupon(
         long cartId,
         string couponCode
      )
      {
         var cart = _cartRepository
            .QueryRp()
            .Include(x => x.Items)
            .FirstOrDefault(x => x.Id == cartId);

         var cartInfoForCoupon = new CartInfoForCoupon
         {
            Items = cart.Items
               .Select(
                  y =>
                     new CartItemForCoupon
                     {
                        ProductId = y.ProductId,
                        Quantity = y.Quantity
                     }
               )
               .ToList()
         };

         var couponValidationResult = await _couponService.Validate(
            cart.CustomerId,
            couponCode,
            cartInfoForCoupon
         );
         if (couponValidationResult.Succeeded)
         {
            cart.CouponCode = couponCode;
            cart.CouponRuleName = couponValidationResult.CouponRuleName;
            _cartItemRepository.SaveChangesRp();
         }

         return couponValidationResult;
      }

      public async Task MigrateCart(long fromUserId, long toUserId)
      {
         var cartFrom = await GetActiveCart(fromUserId);
         if (cartFrom != null && cartFrom.Items.Any())
         {
            var cartTo = await GetActiveCart(toUserId);
            if (cartTo == null)
            {
               cartTo = new Cart
               {
                  CustomerId = toUserId,
                  CreatedById = toUserId,
                  IsProductPriceIncludeTax = cartFrom.IsProductPriceIncludeTax
               };
               _cartRepository.AddRp(cartTo);
            }

            foreach (var fromItem in cartFrom.Items)
            {
               var toItem = cartTo.Items.FirstOrDefault(
                  x => x.ProductId == fromItem.ProductId
               );
               if (toItem == null)
               {
                  toItem = new CartItem
                  {
                     Cart = cartTo,
                     ProductId = fromItem.ProductId,
                     Quantity = fromItem.Quantity,
                     CreatedOn = DateTimeOffset.Now
                  };
                  cartTo.Items.Add(toItem);
               }
               else
               {
                  toItem.Quantity = toItem.Quantity + fromItem.Quantity;
               }
            }

            await _cartRepository.SaveChangesRpAsync();
         }
      }

      public async Task UnlockCart(Cart cart)
      {
         if (cart == null)
         {
            throw new ArgumentNullException(nameof(cart));
         }

         if (cart.LockedOnCheckout)
         {
            cart.LockedOnCheckout = false;
            await _cartRepository.SaveChangesRpAsync();
         }
      }
   }
}
