using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Catalog.Services;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Areas.Catalog.Controllers
{
   [Authorize(
      AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
      Roles = "admin"
   )]
   [ApiController]
   [Area("Catalog")]
   [Route("api/category-admin")]
   public class CategoryAdminController : ControllerBase
   {
      private readonly IRepository<Category> _categoryRepository;
      private readonly IRepository<ProductCategory> _productCategoryRepository;
      private readonly ICategoryService _categoryService;
      private readonly IMediaService _mediaService;

      public CategoryAdminController(
         IRepository<Category> categoryRepository,
         IRepository<ProductCategory> productCategoryRepository,
         ICategoryService categoryService,
         IMediaService mediaService
      )
      {
         _categoryRepository = categoryRepository;
         _productCategoryRepository = productCategoryRepository;
         _categoryService = categoryService;
         _mediaService = mediaService;
      }

      [HttpGet]
      public async Task<IActionResult> GetCategories()
      {
         var gridData = await _categoryService.GetAll();

         return Ok(gridData);
      }
   }
}
