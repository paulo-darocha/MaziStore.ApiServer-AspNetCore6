using System.Collections.Generic;

namespace MaziStore.Module.Cms.Areas.Cms.ViewModels
{
   public class CarouselWidgetVm
   {
      public long Id { get; set; }

      public int DataInterval { get; set; } = 6000;

      public IList<CarouselWidgetItemVm> Items { get; set; } =
         new List<CarouselWidgetItemVm>();
   }
}
