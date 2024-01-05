using MaziStore.Module.Cms.Areas.Cms.ViewModels;
using MaziStore.Module.Core.Areas.Core.ViewModels;
using MaziStore.Module.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;

namespace MaziStore.Module.Cms.Areas.Cms.Controllers
{
   [ApiController]
   [Area("Cms")]
   [Route("api/[controller]")]
   public class CarouselWidgetController : ControllerBase
   {
      private readonly IMediaService _mediaService;

      public CarouselWidgetController(IMediaService mediaService)
      {
         _mediaService = mediaService;
      }

      [HttpPost]
      public IActionResult GetCarouselData(
         [FromBody] WidgetInstanceViewModel widgetInstance
      )
      {
         if (widgetInstance == null)
         {
            return BadRequest("Widget cannot be null.");
         }

         var model = new CarouselWidgetVm
         {
            Id = widgetInstance.Id,
            Items = JsonSerializer.Deserialize<IList<CarouselWidgetItemVm>>(
               widgetInstance.Data
            )
         };

         foreach (var item in model.Items)
         {
            item.Image = _mediaService.GetMediaUrl(item.Image);
         }

         return Ok(model);
      }
   }
}
