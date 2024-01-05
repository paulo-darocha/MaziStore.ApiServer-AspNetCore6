using MaziStore.Module.Pricing.Services;
using MaziStore.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using MaziStore.Module.ShoppingCart.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.ShoppingCart.Services
{
   public interface ICartService
   {
      Task<AddToCartResult> AddToCart(
         long customerId,
         long productId,
         int quantity
      );

      Task<AddToCartResult> AddToCart(
         long customerId,
         long createdById,
         long productId,
         int quantity
      );

      IQueryable<Cart> Query();

      Task<Cart> GetActiveCart(long customerId);

      Task<Cart> GetActiveCart(long customerId, long createdById);

      Task<CartVm> GetActiveCartDetails(long customerId);

      Task<CartVm> GetActiveCartDetails(long customerId, long createdById);

      Task<CouponValidationResult> ApplyCoupon(long cartId, string couponCode);

      Task MigrateCart(long fromUserId, long toUserId);

      Task UnlockCart(Cart cart);
   }
}
