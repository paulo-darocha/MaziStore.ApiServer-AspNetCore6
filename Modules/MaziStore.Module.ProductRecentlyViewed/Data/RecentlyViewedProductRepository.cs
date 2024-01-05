using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Data;
using MaziStore.Module.ProductRecentlyViewed.Models;
using System.Linq;

namespace MaziStore.Module.ProductRecentlyViewed.Data
{
   public class RecentlyViewedProductRepository
      : Repository<Product>,
         IRecentlyViewedProductRepository
   {
      private const long EntityViewedActivityTypeId = 1;
      private const long ProductEntityTypeId = 3;

      public RecentlyViewedProductRepository(MaziStoreDbContext context)
         : base(context) { }

      public IQueryable<Product> GetRecentlyViewedProduct(long userId)
      {
         return from product in DbSetRp
            join rvp in DbContextRp.Set<RecentlyViewedProduct>()
               on product.Id equals rvp.ProductId
            where rvp.UserId == userId
            orderby rvp.LatestViewedOn descending
            select product;
      }
   }
}
