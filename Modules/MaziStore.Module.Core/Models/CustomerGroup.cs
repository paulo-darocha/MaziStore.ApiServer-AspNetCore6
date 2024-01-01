using MaziStore.Module.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace MaziStore.Module.Core.Models
{
   public class CustomerGroup : EntityBase
   {
      public CustomerGroup()
      {
         CreatedOn = DateTimeOffset.Now;
         LatestUpdatedOn = DateTimeOffset.Now;
      }

      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Name { get; set; }

      public string Description { get; set; }

      public bool IsActive { get; set; }

      public bool IsDeleted { get; set; }

      public DateTimeOffset CreatedOn { get; set; }

      public DateTimeOffset LatestUpdatedOn { get; set; }

      public IList<CustomerGroupUser> Users { get; set; } =
         new List<CustomerGroupUser>();
   }
}
