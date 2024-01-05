using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MaziStore.Module.Core.Data;
using MaziStore.Module.Core.Models;

namespace MaziStore.ApiServer.Home.Extensions.Identity
{
    public class MaziUserStore
       : UserStore<
          User,
          Role,
          MaziStoreDbContext,
          long,
          IdentityUserClaim<long>,
          UserRole,
          IdentityUserLogin<long>,
          IdentityUserToken<long>,
          IdentityRoleClaim<long>
       >
    {
        public MaziUserStore(
           MaziStoreDbContext context,
           IdentityErrorDescriber describer
        )
           : base(context, describer) { }
    }
}
