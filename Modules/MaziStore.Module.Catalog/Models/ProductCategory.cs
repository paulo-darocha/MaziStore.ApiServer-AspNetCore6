using MaziStore.Module.Infrastructure.Models;

namespace MaziStore.Module.Catalog.Models
{
   public class ProductCategory : EntityBase
   {
      public bool IsFeaturedProduct { get; set; }

      public int DisplayOrder { get; set; }

      public long CategoryId { get; set; }

      public long ProductId { get; set; }

      public Category Category { get; set; }

      public Product Product { get; set; }
   }
}
