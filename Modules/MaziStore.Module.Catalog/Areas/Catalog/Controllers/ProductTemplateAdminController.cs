using MaziStore.Module.Catalog.Data;
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
      Roles = "Admin"
   )]
   [ApiController]
   [Area("Catalog")]
   [Route("api/product-template-admin")]
   public class ProductTemplateAdminController : ControllerBase
   {
      private readonly IRepository<ProductTemplate> _productTemplateRepository;
      private readonly IRepository<ProductAttribute> _productAttributeRepository;
      private readonly IProductTemplateProductAttributeRepository _productTemplateProductAttributeRepository;

      public ProductTemplateAdminController(
         IRepository<ProductTemplate> productTemplateRepository,
         IRepository<ProductAttribute> productAttributeRepository,
         IProductTemplateProductAttributeRepository productTemplateProductAttributeRepository
      )
      {
         _productTemplateRepository = productTemplateRepository;
         _productAttributeRepository = productAttributeRepository;
         _productTemplateProductAttributeRepository =
            productTemplateProductAttributeRepository;
      }

      [HttpGet]
      public IActionResult GetProductTemplates()
      {
         var productTemplates = _productAttributeRepository
            .QueryRp()
            .Select(x => new { x.Id, x.Name });

         return Ok(productTemplates);
      }
   }
}
