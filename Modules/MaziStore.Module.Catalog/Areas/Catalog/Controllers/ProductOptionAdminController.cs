using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [Authorize(
      AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
      Roles = "admin"
   )]
   [ApiController]
   [Area("Catalog")]
   [Route("api/product-option-admin")]
   public class ProductOptionAdminController : ControllerBase
   {
      private readonly IRepository<ProductOption> _productOptionRepository;

      public ProductOptionAdminController(
         IRepository<ProductOption> productOptionRepository
      )
      {
         _productOptionRepository = productOptionRepository;
      }

      [HttpGet]
      public IActionResult GetProductOptions()
      {
         var options = _productOptionRepository.QueryRp();
         return Ok(options);
      }
   }
}
