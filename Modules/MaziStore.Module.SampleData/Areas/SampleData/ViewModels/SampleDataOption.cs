using System.Collections.Generic;

namespace MaziStore.Module.SampleData.Areas.SampleData.ViewModels
{
   public class SampleDataOption
   {
      public string Industry { get; set; }

      public IList<string> AvailableIndustries { get; set; } = new List<string>();
   }
}
