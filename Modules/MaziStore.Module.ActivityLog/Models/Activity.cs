using MaziStore.Module.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.ActivityLog.Models
{
   public class Activity : EntityBase
   {
      public long ActivityTypeId { get; set; }

      public ActivityType ActivityType { get; set; }

      public long UserId { get; set; }

      public DateTimeOffset CreatedOn { get; set; }

      public long EntityId { get; set; }

      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string EntityTypeId { get; set; }
   }
}
