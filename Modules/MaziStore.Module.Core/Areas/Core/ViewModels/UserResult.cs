namespace MaziStore.Module.Core.Areas.Core.ViewModels
{
   public class UserResult
   {
      public long Id { get; set; }

      public bool Succeeded { get; set; }

      public string Message { get; set; }

      public string Token { get; set; }

      public string FullName { get; set; }

      public string Email { get; set; }

      public string ImageUrl { get; set; }
   }
}
