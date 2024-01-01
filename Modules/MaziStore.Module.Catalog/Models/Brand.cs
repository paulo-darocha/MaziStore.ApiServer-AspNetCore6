using MaziStore.Module.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Catalog.Models
{
   public class Brand : EntityBase
   {
      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Name { get; set; }

      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Slug { get; set; }

      public string Description { get; set; }

      public bool IsPublished { get; set; }

      public bool IsDeleted { get; set; }
   }
}
