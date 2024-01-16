using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.PublicComments.Areas.PublicComments.ViewModels
{
   public class PublicCommentVm
   {
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
