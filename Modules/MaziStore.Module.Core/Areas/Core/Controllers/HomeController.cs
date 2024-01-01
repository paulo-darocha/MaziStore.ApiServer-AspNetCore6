using MaziStore.Module.Core.Areas.Core.ViewModels;
using MaziStore.Module.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MaziStore.Module.Core.Areas.Core.Controllers
{
   [ApiController]
   [Area("Core")]
   [Route("api/[controller]")]
   public class HomeController : ControllerBase
   {
      private readonly ILogger _logger;
      private readonly IWidgetInstanceService _widgetInstanceService;

      public HomeController(
         ILoggerFactory logger,
         IWidgetInstanceService widgetInstanceService
      )
      {
         _logger = logger.CreateLogger("Unhadled Error");
         _widgetInstanceService = widgetInstanceService;
      }

      [HttpGet("")]
      public IActionResult Index()
      {
         var model = new HomeViewModel();

         model.WidgetInstances = _widgetInstanceService
            .GetPublished()
            .OrderBy(x => x.DisplayOrder)
            .Select(
               x =>
                  new WidgetInstanceViewModel
                  {
                     Id = x.Id,
                     Name = x.Name,
                     ViewComponentName = x.Widget.ViewComponentName,
                     WidgetId = x.WidgetId,
                     WidgetZoneId = x.WidgetZoneId,
                     Data = x.Data,
                     HtmlData = x.HtmlData
                  }
            )
            .ToList();

         return Ok(model);
      }
   }
}
