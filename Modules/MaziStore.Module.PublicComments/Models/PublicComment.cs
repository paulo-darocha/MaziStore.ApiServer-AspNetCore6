using MaziStore.Module.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.PublicComments.Models
{
   public class PublicComment : EntityBase
   {
      public int PublicCommentId { get; set; }

      [MaxLength(1000)]
      public string Comment { get; set; }

      [MaxLength(200)]
      public string Title { get; set; }

      [MaxLength(200)]
      public string Name { get; set; }

      [MaxLength(200)]
      public string Email { get; set; }
   }
}
