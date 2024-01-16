using MaziStore.Module.Core.Areas.Core.ViewModels;
using MaziStore.Module.Core.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Areas.Core.Controllers
{
   [ApiController]
   [Area("Core")]
   [Route("api/[controller]")]
   public class AccountController : ControllerBase
   {
      private readonly UserManager<User> _userManager;
      private readonly SignInManager<User> _signInManager;
      private readonly IEmailSender _emailSender;
      private readonly ILogger _logger;
      private IConfiguration _configuration;
      private readonly IRepository<User> _userRepository;

      public AccountController(
         UserManager<User> userManager,
         SignInManager<User> signInManager,
         IEmailSender emailSender,
         ILoggerFactory loggerFactory,
         IConfiguration configuration,
         IRepository<User> userRepository
      )
      {
         _userManager = userManager;
         _signInManager = signInManager;
         _emailSender = emailSender;
         _logger = loggerFactory.CreateLogger<AccountController>();
         _configuration = configuration;
         _userRepository = userRepository;
      }

      [HttpPost("register")]
      public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
      {
         if (ModelState.IsValid)
         {
            var user = new User
            {
               UserName = model.Email,
               Email = model.Email,
               FullName = model.FullName,
               UserGuid = Guid.NewGuid()
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
               result = await _userManager.AddToRoleAsync(user, "customer");
            }

            if (result.Succeeded)
            {
               return Ok($"User '{model.FullName}' created successfully.");
            }

            foreach (var err in result.Errors)
            {
               ModelState.AddModelError("erro", err.Description);
            }
         }

         return BadRequest(ModelState);
      }

      [HttpPost("login")]
      public async Task<IActionResult> Login([FromBody] LoginViewModel model)
      {
         if (ModelState.IsValid)
         {
            var result = await _signInManager.PasswordSignInAsync(
               model.Email,
               model.Password,
               model.RememberMe,
               false
            );

            if (result.Succeeded)
            {
               User user = await _userManager.FindByEmailAsync(model.Email);

               await _signInManager.ClaimsFactory.CreateAsync(user);

               SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
               {
                  Subject = (
                     await _signInManager.CreateUserPrincipalAsync(user)
                  ).Identities.First(),
                  Expires = DateTime.Now.AddMinutes(
                     int.Parse(_configuration["BearerTokens:ExpiryMins"])
                  ),
                  SigningCredentials = new SigningCredentials(
                     new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["BearerTokens:Key"])
                     ),
                     SecurityAlgorithms.HmacSha256Signature
                  ),
               };

               JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
               SecurityToken securityToken =
                  new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

               var userResult = new UserResult
               {
                  Id = user.Id,
                  Succeeded = true,
                  Message = "User logged in successfully.",
                  Token = tokenHandler.WriteToken(securityToken),
                  FullName = user.FullName,
                  Email = user.Email
               };

               return Ok(userResult);
            }
         }
         return BadRequest(ModelState);
      }

      [HttpGet("logout")]
      public async Task<IActionResult> Logout()
      {
         await _signInManager.SignOutAsync();
         return Ok();
      }

      [HttpGet]
      public async Task<IActionResult> CheckIfLogged()
      {
         if (User.Identity.Name is not null)
         {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (await _userManager.IsInRoleAsync(user, "customer"))
            {
               return Ok(new { Logged = true, User = user });
            }
         }

         return Ok(false);
      }
   }
}
