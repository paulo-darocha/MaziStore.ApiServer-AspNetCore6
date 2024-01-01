using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System;

namespace MaziStore.Module.Reviews.Models
{
   public class Reply : EntityBase
   {
      public Reply()
      {
         Status = ReplyStatus.Pending;
         CreatedOn = DateTimeOffset.Now;
      }

      public long ReviewId { get; set; }

      public Review Review { get; set; }

      public long UserId { get; set; }

      public User User { get; set; }

      public string Comment { get; set; }

      [StringLength(450)]
      public string ReplierName { get; set; }

      public ReplyStatus Status { get; set; }

      public DateTimeOffset CreatedOn { get; set; }
   }
}
