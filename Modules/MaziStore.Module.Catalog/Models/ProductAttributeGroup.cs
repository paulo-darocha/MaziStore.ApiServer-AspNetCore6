using MaziStore.Module.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Catalog.Models
{
   public class ProductAttributeGroup : EntityBase
   {
      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Name { get; set; }

      public IList<ProductAttribute> Attributes { get; set; } =
         new List<ProductAttribute>();
   }
}
