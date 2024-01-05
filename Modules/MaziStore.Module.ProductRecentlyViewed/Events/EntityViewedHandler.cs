using MaziStore.Module.Core.Events;
using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.ProductRecentlyViewed.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaziStore.Module.ProductRecentlyViewed.Events
{
   public class EntityViewedHandler : INotificationHandler<EntityViewed>
   {
      private const string ProductEntityTypeId = "Product";
      private readonly IRepository<RecentlyViewedProduct> _recentlyViewedProductRepository;
      private readonly IWorkContext _wordContext;

      public EntityViewedHandler(
         IRepository<RecentlyViewedProduct> recentlyViewedProductRepository,
         IWorkContext wordContext
      )
      {
         _recentlyViewedProductRepository = recentlyViewedProductRepository;
         _wordContext = wordContext;
      }

      public async Task Handle(
         EntityViewed notification,
         CancellationToken cancellationToken
      )
      {
         var user = await _wordContext.GetCurrentUser();
         var recentlyViewedProduct = _recentlyViewedProductRepository
            .QueryRp()
            .FirstOrDefault(
               x => x.ProductId == notification.EntityId && x.UserId == user.Id
            );

         if (recentlyViewedProduct == null)
         {
            recentlyViewedProduct = new RecentlyViewedProduct
            {
               UserId = user.Id,
               ProductId = notification.EntityId,
               LatestViewedOn = DateTimeOffset.Now
            };

            _recentlyViewedProductRepository.AddRp(recentlyViewedProduct);
         }

         recentlyViewedProduct.LatestViewedOn = DateTimeOffset.Now;
         _recentlyViewedProductRepository.SaveChangesRp();
      }
   }
}
