using MaziStore.Module.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Catalog.Models
{
   public class ProductOptionValue : EntityBase
   {
      public long OptionId { get; set; }

      public ProductOption Option { get; set; }

      public long ProductId { get; set; }

      public Product Product { get; set; }

      [StringLength(450)]
      public string Value { get; set; }

      [StringLength(450)]
      public string DisplayType { get; set; }

      public int SortIndex { get; set; }
   }
}
