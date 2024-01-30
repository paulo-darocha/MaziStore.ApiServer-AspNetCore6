using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Catalog.Services;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [Authorize(
      AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
      Roles = "admin"
   )]
   [ApiController]
   [Area("Catalog")]
   [Route("api/brand-admin")]
   public class BrandAdminController : ControllerBase
   {
      private readonly IRepository<Brand> _brandRepository;
      private readonly IBrandService _brandService;

      public BrandAdminController(
         IRepository<Brand> brandRepository,
         IBrandService brandService
      )
      {
         _brandRepository = brandRepository;
         _brandService = brandService;
      }

      [HttpGet]
      public async Task<ActionResult<List<BrandVm>>> GetBrands()
      {
         var brands = await _brandRepository
            .QueryRp()
            .Where(x => !x.IsDeleted)
            .Select(
               x =>
                  new BrandVm
                  {
                     Id = x.Id,
                     Name = x.Name,
                     Slug = x.Slug,
                     IsPublished = x.IsPublished
                  }
            )
            .ToListAsync();

         return brands;
      }
   }
}
