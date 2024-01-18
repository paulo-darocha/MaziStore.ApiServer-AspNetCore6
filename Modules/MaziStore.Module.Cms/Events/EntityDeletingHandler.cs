using MaziStore.Module.Cms.Models;
using MaziStore.Module.Core.Events;
using MaziStore.Module.Infrastructure.Data;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaziStore.Module.Cms.Events
{
   public class EntityDeletingHandler : INotificationHandler<EntityDeleting>
   {
      private readonly IRepository<MenuItem> _menuItemRepository;

      public EntityDeletingHandler(IRepository<MenuItem> menuItemRepository)
      {
         _menuItemRepository = menuItemRepository;
      }

      public Task Handle(
         EntityDeleting notification,
         CancellationToken cancellationToken
      )
      {
         var items = _menuItemRepository
            .QueryRp()
            .Where(x => x.EntityId == notification.EntityId)
            .ToList();
         foreach (var item in items)
         {
            _menuItemRepository.RemoveRp(item);
         }

         return Task.CompletedTask;
      }
   }
}
