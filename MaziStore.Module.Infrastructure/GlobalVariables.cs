using MaziStore.Module.Infrastructure.Modules;
using System.Collections.Generic;

namespace MaziStore.Module.Infrastructure
{
   public static class GlobalVariables
   {
      public static IList<ModuleInfo> Modules { get; set; } =
         new List<ModuleInfo>();

      public static string WebRootPath { get; set; }

      public static string ContentRootPath { get; set; }

      public static string DefaultCulture { get; set; } = "pt-BR";
   }
}
