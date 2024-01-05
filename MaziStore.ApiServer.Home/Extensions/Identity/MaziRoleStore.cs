using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MaziStore.Module.Core.Data;
using MaziStore.Module.Core.Models;

namespace MaziStore.ApiServer.Home.Extensions.Identity
{
    public class MaziRoleStore
       : RoleStore<Role, MaziStoreDbContext, long, UserRole, IdentityRoleClaim<long>>
    {
        public MaziRoleStore(MaziStoreDbContext context)
           : base(context) { }
    }
}
