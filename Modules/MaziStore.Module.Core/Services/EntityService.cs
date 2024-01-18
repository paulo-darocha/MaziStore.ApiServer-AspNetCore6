using MaziStore.Module.Core.Events;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using MediatR;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Services
{
   public class EntityService : IEntityService
   {
      private readonly IRepository<Entity> _entityRepository;
      private readonly IMediator _mediator;

      public EntityService(IRepository<Entity> entityRepository, IMediator mediator)
      {
         _entityRepository = entityRepository;
         _mediator = mediator;
      }

      public string ToSafeSlug(string slug, long entityId, string entityTypeId)
      {
         var i = 2;
         while (true)
         {
            var entity = _entityRepository
               .QueryRp()
               .FirstOrDefault(x => x.Slug == slug);
            if (
               entity != null
               && !(
                  entity.EntityId == entityId && entity.EntityTypeId == entityTypeId
               )
            )
            {
               slug = string.Format("{0}-{1}", slug, i);
               i++;
            }
            else
            {
               break;
            }
         }

         return slug;
      }

      public Entity Get(long entityId, string entityTypeId)
      {
         return _entityRepository
            .QueryRp()
            .FirstOrDefault(
               x => x.EntityId == entityId && x.EntityTypeId == entityTypeId
            );
      }

      public void Add(string name, string slug, long entityId, string entityTypeId)
      {
         var entity = new Entity
         {
            Name = name,
            Slug = slug,
            EntityId = entityId,
            EntityTypeId = entityTypeId
         };

         _entityRepository.AddRp(entity);
      }

      public void Update(
         string newName,
         string newSlug,
         long entityId,
         string entityTypeId
      )
      {
         var entity = _entityRepository
            .QueryRp()
            .First(x => x.EntityId == entityId && x.EntityTypeId == entityTypeId);
         entity.Name = newName;
         entity.Slug = newSlug;
      }

      public async Task Remove(long entityId, string entityTypeId)
      {
         var entity = _entityRepository
            .QueryRp()
            .FirstOrDefault(
               x => x.EntityId == entityId && x.EntityTypeId == entityTypeId
            );

         if (entity != null)
         {
            await _mediator.Publish(new EntityDeleting { EntityId = entityId });
            _entityRepository.RemoveRp(entity);
         }
      }
   }
}
