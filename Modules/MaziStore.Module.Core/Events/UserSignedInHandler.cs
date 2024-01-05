using MaziStore.Module.Core.Extensions;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Events
{
   public class UserSignedInHandler : INotificationHandler<UserSignedIn>
   {
      private readonly IWorkContext _workContext;
      private readonly IRepositoryWithTypedId<User, long> _userRepository;

      public UserSignedInHandler(
         IWorkContext workContext,
         IRepositoryWithTypedId<User, long> userRepository
      )
      {
         _workContext = workContext;
         _userRepository = userRepository;
      }

      public async Task Handle(
         UserSignedIn user,
         CancellationToken cancellationToken
      )
      {
         var guestUser = await _workContext.GetCurrentUser();
         var signedInUser = await _userRepository
            .QueryRp()
            .SingleAsync(x => x.Id == user.UserId);
         signedInUser.Culture = guestUser.Culture;
         await _userRepository.SaveChangesRpAsync();
      }
   }
}
