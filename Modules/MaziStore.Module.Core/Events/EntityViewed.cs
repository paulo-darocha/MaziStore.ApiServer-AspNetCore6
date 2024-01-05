using MediatR;

namespace MaziStore.Module.Core.Events
{
   public class EntityViewed : INotification
   {
      public long EntityId { get; set; }

      public string EntityTypeId { get; set; }
   }
}
