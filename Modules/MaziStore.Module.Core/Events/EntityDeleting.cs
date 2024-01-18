using MediatR;

namespace MaziStore.Module.Core.Events
{
   public class EntityDeleting : INotification
   {
      public long EntityId { get; set; }
   }
}
