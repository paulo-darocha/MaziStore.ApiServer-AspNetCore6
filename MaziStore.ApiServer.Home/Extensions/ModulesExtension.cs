using MaziStore.Module.Infrastructure;
using MaziStore.Module.Infrastructure.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace MaziStore.ApiServer.Home.Extensions
{
   public static class ModulesExtension
   {
      private static readonly IModuleConfigurationManager _modulesConfig =
         new ModuleConfigurationManager();

      public static IServiceCollection AddMaziModules(this IServiceCollection services)
      {
         foreach (var module in _modulesConfig.GetModules())
         {
            if (!module.IsBundledWithHost)
            {
               TryLoadModuleAssembly(module.Id, module);

               if (module.Assembly == null)
               {
                  throw new Exception(
                     $"Cannot find main assembly for module {module.Id}"
                  );
               }
            }
            else
            {
               module.Assembly = Assembly.Load(new AssemblyName(module.Id));
            }

            GlobalVariables.Modules.Add(module);
         }

         return services;
      }

      // ///////////////////////////////////////////////////

      private static void TryLoadModuleAssembly(
         string moduleFolderPath,
         ModuleInfo module
      )
      {
         const string binFolderName = "bin";
         var binFolderPath = Path.Combine(moduleFolderPath, binFolderName);
         var binFolder = new DirectoryInfo(binFolderPath);

         if (Directory.Exists(binFolderPath))
         {
            foreach (
               var file in binFolder.GetFileSystemInfos(
                  "*.dll",
                  SearchOption.AllDirectories
               )
            )
            {
               Assembly assembly;
               try
               {
                  assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(
                     file.FullName
                  );
               }
               catch (FileLoadException)
               {
                  assembly = Assembly.Load(
                     new AssemblyName(Path.GetFileNameWithoutExtension(file.Name))
                  );
                  if (assembly == null)
                  {
                     throw;
                  }

                  string loadedAssemblyVersion = FileVersionInfo
                     .GetVersionInfo(assembly.Location)
                     .FileVersion;
                  string tryToLoadAssemblyVersion = FileVersionInfo
                     .GetVersionInfo(file.FullName)
                     .FileVersion;

                  if (tryToLoadAssemblyVersion != loadedAssemblyVersion)
                  {
                     throw new Exception(
                        $"Cannot load {file.FullName}{tryToLoadAssemblyVersion} "
                           + $" because {assembly.Location}{loadedAssemblyVersion} "
                           + $" has been loaded."
                     );
                  }
               }

               if (
                  Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name)
                  == module.Id
               )
               {
                  module.Assembly = assembly;
               }
            }
         }
      }
   }
}
