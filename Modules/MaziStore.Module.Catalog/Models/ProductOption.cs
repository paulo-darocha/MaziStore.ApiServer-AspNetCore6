using MaziStore.Module.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Catalog.Models
{
   public class ProductOption : EntityBase
   {
      public ProductOption() { }

      public ProductOption(long id)
      {
         Id = id;
      }

      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Name { get; set; }
   }
}
