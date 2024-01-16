using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Infrastructure.Web.SmartTable;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Areas.Core.Controllers
{
   [Authorize(
      AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
      Roles = "admin"
   )]
   [ApiController]
   [Area("Core")]
   [Route("api/[controller]")]
   public class UsersController : ControllerBase
   {
      private readonly IRepository<User> _userRepository;
      private readonly UserManager<User> _userManager;

      public UsersController(
         IRepository<User> userRepository,
         UserManager<User> userManager
      )
      {
         _userRepository = userRepository;
         _userManager = userManager;
      }

      [HttpPost("grid")]
      public IActionResult List([FromBody] SmartTableParam param)
      {
         var query = _userRepository
            .QueryRp()
            .Include(x => x.Roles)
            .ThenInclude(y => y.Role)
            .Include(x => x.CustomerGroups)
            .ThenInclude(y => y.CustomerGroup)
            .Where(x => !x.IsDeleted);

         if (
            param.Search.PredicateObject != null
         )
         {
            dynamic search = param.Search.PredicateObject;

            if (search.Email != null)
            {
               string email = search.Email;
               query = query.Where(x => x.Email.Contains(email));
            }

            if (search.FullName != null)
            {
               string fullName = search.FullName;
               query = query.Where(x => x.FullName.Contains(fullName));
            }

            if (search.RoleId != null)
            {
               long roleId = search.RoleId;
               query = query.Where(x => x.Roles.Any(y => y.RoleId == roleId));
            }

            if (search.CustomerGroupId != null)
            {
               long customerGroupId = search.CustomerGroupId;
               query = query.Where(
                  x =>
                     x.CustomerGroups.Any(y => y.CustomerGroupId == customerGroupId)
               );
            }

            if (search.CreatedOn != null)
            {
               if (search.CreatedOn.before != null)
               {
                  DateTimeOffset before = search.CreatedOn.before;
                  query = query.Where(x => x.CreatedOn <= before);
               }

               if (search.CreatedOn.after != null)
               {
                  DateTimeOffset after = search.CreatedOn.after;
                  query = query.Where(x => x.CreatedOn >= after);
               }
            }
         }

         var users = query.ToSmartTableResultNoProjection(
            param,
            user =>
               new
               {
                  user.Id,
                  user.Email,
                  user.FullName,
                  user.CreatedOn,
                  Roles = user.Roles.Select(x => x.Role.Name),
                  CustomerGroups = user.CustomerGroups.Select(
                     x => x.CustomerGroup.Name
                  )
               }
         );

         return Ok(users);
      }

      [HttpDelete("{id}")]
      public async Task<IActionResult> Delete(long id)
      {
         var user = await _userRepository
            .QueryRp()
            .FirstOrDefaultAsync(x => x.Id == id);

         if (user == null)
         {
            return NotFound($"User '{id}' not found");
         }

         user.IsDeleted = true;
         user.LockoutEnabled = true;
         user.LockoutEnd = DateTime.Now.AddYears(200);
         await _userRepository.SaveChangesRpAsync();
         return Ok();
      }
   }
}
