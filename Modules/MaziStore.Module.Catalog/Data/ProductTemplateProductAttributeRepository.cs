using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Catalog.Data
{
   public class ProductTemplateProductAttributeRepository
      : IProductTemplateProductAttributeRepository
   {
      private readonly DbContext dbContext;

      public ProductTemplateProductAttributeRepository(MaziStoreDbContext dbContext)
      {
         this.dbContext = dbContext;
      }

      public void Remove(ProductTemplateProductAttribute item)
      {
         dbContext.Set<ProductTemplateProductAttribute>().Remove(item);
      }
   }
}
