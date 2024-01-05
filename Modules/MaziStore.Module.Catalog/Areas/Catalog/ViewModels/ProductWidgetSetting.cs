using System.Text.Json.Serialization;

namespace MaziStore.Module.Catalog.Areas.Catalog.ViewModels
{
   public class ProductWidgetSetting
   {
      public int NumberOfProducts { get; set; }

      public long? CategoryId { get; set; }

      [JsonConverter(typeof(JsonStringEnumConverter))]
      public ProductWidgetOrderBy OrderBy { get; set; }

      public bool FeaturedOnly { get; set; }
   }
}
