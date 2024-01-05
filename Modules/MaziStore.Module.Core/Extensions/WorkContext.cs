using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Extensions
{
   public class WorkContext : IWorkContext
   {
      private const string MaziCookie = "MaziCookie";
      private const long GuestRoleId = 3;

      private User _currentUser;
      private UserManager<User> _userManager;
      private HttpContext _httpContext;
      private IRepository<User> _userRepository;
      private readonly IConfiguration _configuration;

      public WorkContext(
         UserManager<User> userManager,
         IHttpContextAccessor httpContextAccessor,
         IRepository<User> userRepository,
         IConfiguration configuration
      )
      {
         _userManager = userManager;
         _httpContext = httpContextAccessor.HttpContext;
         _userRepository = userRepository;
         _configuration = configuration;
      }

      public async Task<User> GetCurrentUser()
      {
         if (_currentUser != null)
         {
            Console.WriteLine($"\nUser 1st try: {_currentUser}\n");
            return _currentUser;
         }

         var contextUser = _httpContext.User;
         _currentUser = await _userManager.GetUserAsync(contextUser);

         if (_currentUser != null)
         {
            Console.WriteLine(
               $"\nUser 2nd try (from UserManager): {_currentUser}\n"
            );
            return _currentUser;
         }

         var userGuid = GetUserGuidFromCookies();
         if (userGuid.HasValue)
         {
            Console.WriteLine($"\nUser 3st Try (Cookie Guid): {userGuid}\n");
            _currentUser = _userRepository
               .QueryRp()
               .Include(x => x.Roles)
               .FirstOrDefault(x => x.UserGuid == userGuid);
         }

         if (
            _currentUser != null
            && _currentUser.Roles.Count == 1
            && _currentUser.Roles.First().RoleId == GuestRoleId
         )
         {
            Console.WriteLine(
               $"\nUser 4th Try (from DB/cookie): {_currentUser.Id}\n"
            );
            return _currentUser;
         }

         Console.WriteLine($"\nUser 5th Try (cookie not worked)\n");
         userGuid = Guid.NewGuid();
         var dummyEmail = string.Format("{0}@guest.mazistore.com", userGuid);
         _currentUser = new User
         {
            FullName = "Guest",
            UserGuid = userGuid.Value,
            Email = dummyEmail,
            UserName = dummyEmail,
            Culture = GlobalVariables.DefaultCulture
         };
         var abc = await _userManager.CreateAsync(_currentUser, "1qazZAQ!");
         await _userManager.AddToRoleAsync(_currentUser, "guest");
         SetUserGuidCookies();
         return _currentUser;
      }

      // ////////////////////////////////////////////////////////////

      private Guid? GetUserGuidFromCookies()
      {
         if (_httpContext.Request.Cookies.ContainsKey(MaziCookie))
         {
            return Guid.Parse(_httpContext.Request.Cookies[MaziCookie]);
         }

         return null;
      }

      private void SetUserGuidCookies()
      {
         _httpContext.Response.Cookies.Append(
            MaziCookie,
            _currentUser.UserGuid.ToString(),
            new CookieOptions
            {
               Expires = DateTime.UtcNow.AddDays(2),
               HttpOnly = true,
               IsEssential = true,
               SameSite = SameSiteMode.None,
               Secure = true
            }
         );
      }
   }
}
