using MaziStore.Module.Core.Events;
using MaziStore.Module.ActivityLog.Models;
using MediatR;
using MaziStore.Module.Infrastructure.Data;
using System.Threading.Tasks;
using System.Threading;
using MaziStore.Module.Core.Extensions;
using System;

namespace MaziStore.Module.ActivityLog.Events
{
   public class EntityViewedHandler : INotificationHandler<EntityViewed>
   {
      private readonly IRepository<Activity> _activityRepository;
      private readonly IWorkContext _workContext;
      private const long EntityViewedActivityTypeId = 1;

      public EntityViewedHandler(
         IRepository<Activity> activityRepository,
         IWorkContext workContext
      )
      {
         _activityRepository = activityRepository;
         _workContext = workContext;
      }

      public async Task Handle(
         EntityViewed notification,
         CancellationToken cancellationToken
      )
      {
         var user = await _workContext.GetCurrentUser();
         var activity = new Activity
         {
            ActivityTypeId = EntityViewedActivityTypeId,
            EntityId = notification.EntityId,
            EntityTypeId = notification.EntityTypeId,
            UserId = user.Id,
            CreatedOn = DateTimeOffset.Now
         };

         _activityRepository.AddRp(activity);
      }
   }
}
