using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Tax.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Tax.Areas.Tax.Controllers
{
   [Authorize(
      AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
      Roles = "admin"
   )]
   [ApiController]
   [Area("tax")]
   [Route("api/tax-class-admin")]
   public class TaxClassAdminController : ControllerBase
   {
      private readonly IRepository<TaxClass> _taxClassRepository;
      private readonly int _defaultTaxClassId;

      public TaxClassAdminController(
         IRepository<TaxClass> taxClassRepository,
         IConfiguration _configuration
      )
      {
         _taxClassRepository = taxClassRepository;
         _defaultTaxClassId = int.Parse(_configuration["Tax:DefaultTaxClassId"]);
      }

      [HttpGet]
      public async Task<IActionResult> Get()
      {
         var taxClasses = await _taxClassRepository
            .QueryRp()
            .Select(x => new { Id = x.Id, Name = x.Name })
            .ToListAsync();

         return Ok(taxClasses);
      }

      [HttpGet("default")]
      public async Task<IActionResult> DefaultTaxClass()
      {
         var defaultTaxClass = await _taxClassRepository
            .QueryRp()
            .Select(x => new { x.Id, x.Name })
            .FirstOrDefaultAsync(x => x.Id == _defaultTaxClassId);

         return Ok(defaultTaxClass);
      }
   }
}
