using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [Authorize(
      AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
      Roles = "admin"
   )]
   [ApiController]
   [Area("Catalog")]
   [Route("api/product-attribute-admin")]
   public class ProductAttributeAdminController : ControllerBase
   {
      private readonly IRepository<ProductAttribute> _productAttributeRepository;

      public ProductAttributeAdminController(
         IRepository<ProductAttribute> productAttributeRepository
      )
      {
         _productAttributeRepository = productAttributeRepository;
      }

      [HttpGet]
      public IActionResult ListAttributes()
      {
         var attributes = _productAttributeRepository
            .QueryRp()
            .Select(
               x =>
                  new
                  {
                     Id = x.Id,
                     Name = x.Name,
                     GroupName = x.Group.Name
                  }
            );

         return Ok(attributes);
      }
   }
}
