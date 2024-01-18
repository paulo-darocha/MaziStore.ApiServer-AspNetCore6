using MaziStore.Module.Infrastructure;
using MaziStore.Module.SampleData.Areas.SampleData.ViewModels;
using MaziStore.Module.SampleData.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Areas.Core.Controllers
{
   [ApiController]
   [Area("SampleData")]
   [Route("api/[controller]")]
   public class SampleDataController : ControllerBase
   {
      private readonly ISampleDataService _sampleDataService;
      private readonly IConfiguration _configuration;

      public SampleDataController(
         ISampleDataService sampleDataService,
         IConfiguration configuration
      )
      {
         _sampleDataService = sampleDataService;
         _configuration = configuration;
      }

      [HttpGet]
      public IActionResult Index()
      {
         //var sampleFolder = Path.Combine(GlobalVariables.ContentRootPath, "Sample");
         var sampleFolder = Path.Combine(
            GlobalVariables.ContentRootPath,
            _configuration["SampleFolder"]
         );
         var directory = new DirectoryInfo(sampleFolder);

         var industries = directory.GetDirectories().Select(x => x.Name).ToList();

         var model = new SampleDataOption { AvailableIndustries = industries };

         return Ok(model);
      }

      [HttpGet("{industry}")]
      public async Task<IActionResult> ResetToSample(string industry)
      {
         var model = new SampleDataOption();
         model.Industry = industry;
         await _sampleDataService.ResetToSampleData(model);
         return Ok();
      }
   }
}
