using MaziStore.Module.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Cms.Models
{
   public class Menu : EntityBase
   {
      public Menu() { }

      public Menu(long id)
      {
         Id = id;
      }

      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Name { get; set; }

      public bool IsPublished { get; set; }

      public bool IsSystem { get; set; }

      public IList<MenuItem> MenuItems { get; protected set; } =
         new List<MenuItem>();
   }
}
