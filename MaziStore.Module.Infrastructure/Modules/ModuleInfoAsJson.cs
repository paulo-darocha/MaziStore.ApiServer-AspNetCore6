using System.Reflection;
using System;

namespace MaziStore.Module.Infrastructure.Modules
{
   public class ModuleInfoAsJson
   {
      [System.Diagnostics.CodeAnalysis.SuppressMessage(
         "Style",
         "IDE1006:Naming Styles",
         Justification = "Class type for Json file"
      )]
      public string id { get; set; }

      [System.Diagnostics.CodeAnalysis.SuppressMessage(
         "Style",
         "IDE1006:Naming Styles",
         Justification = "Class type for Json file"
      )]
      public string name { get; set; }

      [System.Diagnostics.CodeAnalysis.SuppressMessage(
         "Style",
         "IDE1006:Naming Styles",
         Justification = "Class type for Json file"
      )]
      public bool isBundledWithHost { get; set; }

      [System.Diagnostics.CodeAnalysis.SuppressMessage(
         "Style",
         "IDE1006:Naming Styles",
         Justification = "Class type for Json file"
      )]
      public Version version { get; set; }

      [System.Diagnostics.CodeAnalysis.SuppressMessage(
         "Style",
         "IDE1006:Naming Styles",
         Justification = "Class type for Json file"
      )]
      public Assembly assembly { get; set; }
   }
}
