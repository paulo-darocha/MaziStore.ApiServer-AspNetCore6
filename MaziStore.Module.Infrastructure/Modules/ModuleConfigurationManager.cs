using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MaziStore.Module.Infrastructure.Modules
{
   public class ModuleConfigurationManager : IModuleConfigurationManager
   {
      public static readonly string ModulesFile = "modules.json";

      public IEnumerable<ModuleInfo> GetModules()
      {
         var modulesPath = Path.Combine(
            GlobalVariables.ContentRootPath,
            ModulesFile
         );

         using var reader = new StreamReader(modulesPath);

         string content = reader.ReadToEnd();

         IEnumerable<ModuleInfoAsJson> modulesAsJson = JsonSerializer.Deserialize<
            IEnumerable<ModuleInfoAsJson>
         >(content);

         foreach (var module in modulesAsJson)
         {
            yield return new ModuleInfo
            {
               Id = module.id,
               Version = Version.Parse(module.version.ToString()),
               IsBundledWithHost = module.isBundledWithHost
            };
         }
      }
   }
}
