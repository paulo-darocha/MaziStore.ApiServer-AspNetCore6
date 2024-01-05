using MediatR;

namespace MaziStore.Module.Core.Events
{
   public class UserSignedIn : INotification
   {
      public long UserId { get; set; }
   }
}
