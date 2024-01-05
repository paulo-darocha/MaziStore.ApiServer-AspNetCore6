using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Cms.Models
{
   public class MenuItem : EntityBase
   {
      public long? ParentId { get; set; }

      public MenuItem Parent { get; set; }

      public IList<MenuItem> Children { get; protected set; } =
         new List<MenuItem>();

      public long MenuId { get; set; }

      public Menu Menu { get; set; }

      public long? EntityId { get; set; }

      public Entity Entity { get; set; }

      [StringLength(450)]
      public string CustomLink { get; set; }

      [StringLength(450)]
      public string Name { get; set; }

      public int DisplayOrder { get; set; }
   }
}
