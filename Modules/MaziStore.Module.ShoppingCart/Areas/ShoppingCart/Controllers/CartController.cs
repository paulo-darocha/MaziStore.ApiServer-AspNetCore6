using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using MaziStore.Module.ShoppingCart.Models;
using MaziStore.Module.ShoppingCart.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.ShoppingCart.Areas.ShoppingCart.Controllers
{
   [ApiController]
   [Area("ShoppingCart")]
   [Route("api/[controller]")]
   public class CartController : ControllerBase
   {
      private readonly IRepository<CartItem> _cartItemRepository;
      private readonly IRepository<Cart> _cartRepository;
      private readonly ICartService _cartService;
      private readonly IMediaService _mediaService;
      private readonly IWorkContext _workContext;
      private readonly ICurrencyService _currencyService;

      public CartController(
         IRepository<CartItem> cartItemRepository,
         IRepository<Cart> cartRepository,
         ICartService cartService,
         IMediaService mediaService,
         IWorkContext workContext,
         ICurrencyService currencyService
      )
      {
         _cartItemRepository = cartItemRepository;
         _cartRepository = cartRepository;
         _cartService = cartService;
         _mediaService = mediaService;
         _workContext = workContext;
         _currencyService = currencyService;
      }

      [HttpPost("add/{id?}")]
      public async Task<IActionResult> AddToCart(
         [FromBody] AddToCartModel model,
         long id = 0
      )
      {
         Console.WriteLine($"\nID recebido (add): {id}\n");
         if (id == 0)
         {
            id = (await _workContext.GetCurrentUser()).Id;
         }

         var result = await _cartService.AddToCart(
            id,
            model.ProductId,
            model.Quantity
         );

         if (result.Success)
         {
            var cart = await _cartService.GetActiveCartDetails(id);

            var cartResult = new AddToCartResultVm(_currencyService)
            {
               CartItemCount = cart.Items.Count,
               CartAmount = cart.SubTotal
            };

            var addedProduct = cart.Items.First(
               x => x.ProductId == model.ProductId
            );

            cartResult.ProductName = addedProduct.ProductName;
            cartResult.ProductImage = addedProduct.ProductImage;
            cartResult.ProductPrice = addedProduct.ProductPrice;
            cartResult.Quantity = addedProduct.Quantity;

            return Ok(cartResult);
         }
         else
         {
            return BadRequest(result);
         }
      }

      [HttpGet("count/{id}")]
      public async Task<IActionResult> CountItems(long id = 0)
      {
         Console.WriteLine($"\nID recebido (count): {id}\n");
         if (id == 0)
         {
            id = (await _workContext.GetCurrentUser()).Id;
         }

         var cart = await _cartService.GetActiveCartDetails(id);

         if (cart == null)
         {
            cart = new CartVm(_currencyService);
         }

         return Ok(cart.Items.Count);
      }

      [HttpGet("list/{id?}")]
      public async Task<IActionResult> List(long id = 0)
      {
         Console.WriteLine($"\nID recebido (lista): {id}\n");
         if (id == 0)
         {
            id = (await _workContext.GetCurrentUser()).Id;
         }

         var cart = await _cartService.GetActiveCartDetails(id);

         if (cart == null)
         {
            cart = new CartVm(_currencyService);
         }

         return Ok(cart);
      }

      [HttpPost("update/{id?}")]
      public async Task<IActionResult> UpdateQuantity(
         [FromBody] CartQuantityUpdate model,
         long id = 0
      )
      {
         Console.WriteLine($"\nID recebido (update): {id}\n");
         if (model.Quantity <= 0)
         {
            return BadRequest("The quantity must be larger than zero.");
         }

         if (id == 0)
         {
            id = (await _workContext.GetCurrentUser()).Id;
         }

         var cart = await _cartService.GetActiveCart(id);
         if (cart == null)
         {
            return BadRequest($"Cart not found");
         }

         if (cart.LockedOnCheckout)
         {
            return BadRequest("Cart locked for Checkout");
         }

         var cartItem = _cartItemRepository
            .QueryRp()
            .Include(x => x.Product)
            .FirstOrDefault(
               x => x.Id == model.CartItemId && x.Cart.CreatedById == id
            );
         if (cartItem == null)
         {
            return BadRequest("Item from cart not found.");
         }

         if (model.Quantity > cartItem.Quantity)
         {
            if (
               cartItem.Product.StockTrackingIsEnabled
               && cartItem.Product.StockQuantity < model.Quantity
            )
            {
               return BadRequest(
                  $"There are only {cartItem.Product.StockQuantity} "
                     + $"items available for {cartItem.Product.Name}."
               );
            }
         }

         cartItem.Quantity = model.Quantity;
         _cartItemRepository.SaveChangesRp();

         return await List(id);
      }

      [HttpPost("remove/{id?}")]
      public async Task<IActionResult> Remove(
         [FromBody] CartQuantityUpdate model,
         long id
      )
      {
         Console.WriteLine($"\nID recebido (remove): {id}\n");
         if (id == 0)
         {
            id = (await _workContext.GetCurrentUser()).Id;
         }
         var cart = await _cartService.GetActiveCart(id);
         if (cart == null)
         {
            return BadRequest("Cart not found.");
         }

         if (cart.LockedOnCheckout)
         {
            return BadRequest("Cart locked for Checkout");
         }

         var cartItem = _cartItemRepository
            .QueryRp()
            .FirstOrDefault(
               x => x.Id == model.CartItemId && x.Cart.CreatedById == id
            );
         if (cartItem == null)
         {
            return BadRequest("Item from cart not found.");
         }

         _cartItemRepository.RemoveRp(cartItem);
         _cartItemRepository.SaveChangesRp();

         cart = await _cartService.GetActiveCart(id);
         if (cart.Items.Count < 1)
         {
            _cartRepository.RemoveRp(cart);
            _cartRepository.SaveChangesRp();
         }

         return await List(id);
      }
   }
}
