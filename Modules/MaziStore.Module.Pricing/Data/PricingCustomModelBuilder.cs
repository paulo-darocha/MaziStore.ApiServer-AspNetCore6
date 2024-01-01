using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Pricing.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Pricing.Data
{
   public class PricingCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<Coupon>(coupon =>
         {
            coupon
               .HasOne(x => x.CartRule)
               .WithMany(y => y.Coupons)
               .HasForeignKey(x => x.CartRuleId)
               .OnDelete(DeleteBehavior.Cascade);
         });

         modelBuilder.Entity<CartRuleCategory>(cartRuleCat =>
         {
            cartRuleCat.HasKey(x => new { x.CartRuleId, x.CategoryId });
            cartRuleCat
               .HasOne(x => x.CartRule)
               .WithMany(y => y.Categories)
               .HasForeignKey(x => x.CartRuleId)
               .OnDelete(DeleteBehavior.Cascade);
            cartRuleCat
               .HasOne(x => x.Category)
               .WithMany()
               .HasForeignKey(x => x.CategoryId);
         });

         modelBuilder.Entity<CartRuleProduct>(cartRuleProd =>
         {
            cartRuleProd.HasKey(x => new { x.CartRuleId, x.ProductId });
            cartRuleProd
               .HasOne(x => x.CartRule)
               .WithMany(y => y.Products)
               .HasForeignKey(x => x.CartRuleId)
               .OnDelete(DeleteBehavior.Cascade);
            cartRuleProd
               .HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId);
         });

         modelBuilder.Entity<CartRuleCustomerGroup>(cartRuleCustGroup =>
         {
            cartRuleCustGroup.HasKey(x => new { x.CartRuleId, x.CustomerGroupId });
            cartRuleCustGroup
               .HasOne(x => x.CartRule)
               .WithMany(y => y.CustomerGroups)
               .HasForeignKey(x => x.CartRuleId)
               .OnDelete(DeleteBehavior.Cascade);
            cartRuleCustGroup
               .HasOne(x => x.CustomerGroup)
               .WithMany()
               .HasForeignKey(x => x.CustomerGroupId);
         });

         modelBuilder.Entity<CatalogRuleCustomerGroup>(catRuleCustGroup =>
         {
            catRuleCustGroup.HasKey(
               x => new { x.CatalogRuleId, x.CustomerGroupId }
            );
            catRuleCustGroup
               .HasOne(x => x.CatalogRule)
               .WithMany(y => y.CustomerGroups)
               .HasForeignKey(x => x.CatalogRuleId);
            catRuleCustGroup
               .HasOne(x => x.CustomerGroup)
               .WithMany()
               .HasForeignKey(x => x.CustomerGroupId);
         });
      }
   }
}
