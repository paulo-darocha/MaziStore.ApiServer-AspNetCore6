using MaziStore.Module.Catalog.Models;
using System.Linq;

namespace MaziStore.Module.ProductRecentlyViewed.Data
{
   public interface IRecentlyViewedProductRepository
   {
      IQueryable<Product> GetRecentlyViewedProduct(long userId);
   }
}
