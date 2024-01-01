using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;
using System;

namespace MaziStore.Module.ProductComparison.Models
{
   public class ComparingProduct : EntityBase
   {
      public DateTimeOffset CreatedOn { get; set; }

      public long UserId { get; set; }

      public User User { get; set; }

      public long ProductId { get; set; }

      public Product Product { get; set; }
   }
}
