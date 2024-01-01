using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;

namespace MaziStore.Module.Catalog.Models
{
   public class ProductMedia : EntityBase
   {
      public long ProductId { get; set; }

      public Product Product { get; set; }

      public long MediaId { get; set; }

      public Media Media { get; set; }

      public int DisplayOrder { get; set; }
   }
}
