using MaziStore.Module.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Catalog.Models
{
   public class ProductAttribute : EntityBase
   {
      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Name { get; set; }

      public long GroupId { get; set; }

      public ProductAttributeGroup Group { get; set; }

      public IList<ProductTemplateProductAttribute> ProductTemplates
      {
         get;
         protected set;
      } = new List<ProductTemplateProductAttribute>();
   }
}
