using System.Collections.Generic;

namespace MaziStore.Module.Core.Areas.Core.ViewModels
{
   public class HomeViewModel
   {
      public IList<WidgetInstanceViewModel> WidgetInstances { get; set; } =
         new List<WidgetInstanceViewModel>();
   }
}
