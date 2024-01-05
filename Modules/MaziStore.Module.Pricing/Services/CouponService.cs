using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Pricing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Pricing.Services
{
   public class CouponService : ICouponService
   {
      private readonly IRepository<Coupon> _couponRepository;
      private readonly IRepository<CartRuleUsage> _cartRuleUsageRepository;
      private readonly IRepository<Product> _productRepository;

      public CouponService(
         IRepository<Coupon> couponRepository,
         IRepository<CartRuleUsage> cartRuleUsageRepository,
         IRepository<Product> productRepository
      )
      {
         _couponRepository = couponRepository;
         _cartRuleUsageRepository = cartRuleUsageRepository;
         _productRepository = productRepository;
      }

      public async Task<CouponValidationResult> Validate(
         long customerId,
         string couponCode,
         CartInfoForCoupon cart
      )
      {
         var coupon = await _couponRepository
            .QueryRp()
            .Include(x => x.CartRule)
            .ThenInclude(y => y.Products)
            .Include(x => x.CartRule)
            .ThenInclude(y => y.Categories)
            .FirstOrDefaultAsync(x => x.Code == couponCode);

         var validationResult = new CouponValidationResult { Succeeded = false };

         if (coupon == null || !coupon.CartRule.IsActive)
         {
            validationResult.ErrorMessage =
               $"The coupon {couponCode} does not exist";
            return validationResult;
         }

         if (
            coupon.CartRule.StartOn.HasValue
            && coupon.CartRule.StartOn > DateTimeOffset.Now
         )
         {
            validationResult.ErrorMessage =
               $"The coupon {couponCode} should be used after {coupon.CartRule.StartOn}.";
         }

         if (
            coupon.CartRule.EndOn.HasValue
            && coupon.CartRule.EndOn <= DateTimeOffset.Now
         )
         {
            validationResult.ErrorMessage = $"The coupon {couponCode} is expired.";
         }

         var couponUsageCount = _cartRuleUsageRepository
            .QueryRp()
            .Count(x => x.CouponId == coupon.Id);
         if (
            coupon.CartRule.UsageLimitPerCoupon.HasValue
            && couponUsageCount >= coupon.CartRule.UsageLimitPerCoupon
         )
         {
            validationResult.ErrorMessage = $"The coupon {couponCode} is all used.";
            return validationResult;
         }

         var couponUsageByCustomerCount = _cartRuleUsageRepository
            .QueryRp()
            .Count(x => x.CouponId == coupon.Id && x.UserId == customerId);
         if (
            coupon.CartRule.UsageLimitPerCustomer.HasValue
            && couponUsageByCustomerCount >= coupon.CartRule.UsageLimitPerCustomer
         )
         {
            validationResult.ErrorMessage =
               $"You can use the coupon {couponCode} only"
               + $" {coupon.CartRule.UsageLimitPerCustomer} times.";
            return validationResult;
         }

         IList<DiscountableProduct> discountableProducts =
            new List<DiscountableProduct>();

         if (!coupon.CartRule.Products.Any() && !coupon.CartRule.Categories.Any())
         {
            var productIds = cart.Items.Select(x => x.ProductId);
            discountableProducts = _productRepository
               .QueryRp()
               .Where(x => productIds.Contains(x.Id))
               .Select(
                  x =>
                     new DiscountableProduct
                     {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price
                     }
               )
               .ToList();
         }
         else
         {
            discountableProducts = GetDiscountableProduct(
               coupon.CartRule.Products,
               coupon.CartRule.Categories
            );
         }

         foreach (var item in cart.Items)
         {
            if (
               (
                  coupon.CartRule.UsageLimitPerCoupon.HasValue
                  && couponUsageCount >= coupon.CartRule.UsageLimitPerCoupon
               )
               || (
                  coupon.CartRule.UsageLimitPerCustomer.HasValue
                  && couponUsageByCustomerCount
                     >= coupon.CartRule.UsageLimitPerCustomer
               )
            )
            {
               break;
            }

            var discountableProduct = discountableProducts.FirstOrDefault(
               x => x.Id == item.ProductId
            );

            if (discountableProduct != null)
            {
               var discountedProduct = new DiscountedProduct
               {
                  Id = discountableProduct.Id,
                  Name = discountableProduct.Name,
                  Price = discountableProduct.Price,
                  Quantity = 1
               };
               couponUsageCount = couponUsageCount + 1;
               couponUsageByCustomerCount = couponUsageByCustomerCount + 1;

               for (var i = 1; i < item.Quantity; i++)
               {
                  if (
                     (
                        coupon.CartRule.UsageLimitPerCoupon.HasValue
                        && couponUsageCount >= coupon.CartRule.UsageLimitPerCoupon
                     )
                     || (
                        coupon.CartRule.UsageLimitPerCustomer.HasValue
                        && couponUsageByCustomerCount
                           >= coupon.CartRule.UsageLimitPerCustomer
                     )
                  )
                  {
                     break;
                  }

                  discountedProduct.Quantity = discountedProduct.Quantity + 1;
                  couponUsageCount = couponUsageCount + 1;
                  couponUsageByCustomerCount = couponUsageByCustomerCount + 1;
               }

               validationResult.DiscountedProducts.Add(discountedProduct);
            }
         }

         if (!validationResult.DiscountedProducts.Any())
         {
            validationResult.ErrorMessage =
               $"The coupon {couponCode} does not apply to any "
               + $"products in your cart.";
            return validationResult;
         }

         validationResult.Succeeded = true;
         validationResult.CouponId = coupon.Id;
         validationResult.CouponCode = coupon.Code;
         validationResult.CouponRuleName = coupon.CartRule.Name;
         validationResult.CartRule = coupon.CartRule;

         switch (coupon.CartRule.RuleToApply)
         {
            case "cart_fixed":
               validationResult.DiscountAmount = coupon.CartRule.DiscountAmount;
               return validationResult;

            case "by_percent":
               foreach (var item in validationResult.DiscountedProducts)
               {
                  item.DiscountAmount =
                     (item.Price * coupon.CartRule.DiscountAmount / 100)
                     * item.Quantity;
               }
               validationResult.DiscountAmount =
                  validationResult.DiscountedProducts.Sum(x => x.DiscountAmount);
               return validationResult;

            default:
               throw new InvalidOperationException(
                  $"{coupon.CartRule.RuleToApply} is not supported."
               );
         }
      }

      public void AddCouponUsage(
         long customerId,
         long orderId,
         CouponValidationResult couponValidationResult
      )
      {
         if (
            !couponValidationResult.Succeeded
            || couponValidationResult.CartRule == null
         )
         {
            return;
         }

         CartRuleUsage couponUsage;
         switch (couponValidationResult.CartRule.RuleToApply)
         {
            case "cart_fixed":
               couponUsage = new CartRuleUsage
               {
                  UserId = customerId,
                  OrderId = orderId,
                  CouponId = couponValidationResult.CouponId,
                  CartRuleId = couponValidationResult.CartRule.Id
               };
               _cartRuleUsageRepository.AddRp(couponUsage);
               break;

            case "by_percent":
               foreach (var item in couponValidationResult.DiscountedProducts)
               {
                  for (var i = 0; i < item.Quantity; i++)
                  {
                     couponUsage = new CartRuleUsage
                     {
                        UserId = customerId,
                        OrderId = orderId,
                        CouponId = couponValidationResult.CouponId,
                        CartRuleId = couponValidationResult.CartRule.Id
                     };
                     _cartRuleUsageRepository.AddRp(couponUsage);
                  }
               }
               break;

            default:
               throw new InvalidOperationException(
                  $"{couponValidationResult.CartRule.RuleToApply} is not supported."
               );
         }
      }

      // //////////////////////////////////////////////////////////

      private IList<DiscountableProduct> GetDiscountableProduct(
         IList<CartRuleProduct> cartRuleProducts,
         IList<CartRuleCategory> cartRuleCategories
      )
      {
         IList<DiscountableProduct> discountedProducts =
            new List<DiscountableProduct>();
         if (cartRuleProducts.Any())
         {
            var productIds = cartRuleProducts.Select(x => x.ProductId);
            discountedProducts = _productRepository
               .QueryRp()
               .Where(x => productIds.Contains(x.Id))
               .Select(
                  x =>
                     new DiscountableProduct
                     {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price
                     }
               )
               .ToList();
         }

         if (cartRuleCategories.Any())
         {
            var categoriesId = cartRuleCategories.Select(x => x.CategoryId);
            var discountedProductByCategories = _productRepository
               .QueryRp()
               .Where(x => x.Categories.Any(y => categoriesId.Contains(y.Id)))
               .Select(
                  x =>
                     new DiscountableProduct
                     {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price
                     }
               )
               .ToList();
            discountedProducts = discountedProducts
               .Concat(discountedProductByCategories)
               .ToList();
         }

         return discountedProducts;
      }
   }
}
