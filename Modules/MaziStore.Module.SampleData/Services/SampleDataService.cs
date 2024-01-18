using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure;
using MaziStore.Module.SampleData.Areas.SampleData.ViewModels;
using MaziStore.Module.SampleData.Data;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MaziStore.Module.SampleData.Services
{
   public class SampleDataService : ISampleDataService
   {
      private readonly ISqlRepository _sqlRepository;
      private readonly IMediaService _mediaService;
      private readonly IConfiguration _configuration;

      public SampleDataService(
         ISqlRepository sqlRepository,
         IMediaService mediaService,
         IConfiguration configuration
      )
      {
         _sqlRepository = sqlRepository;
         _mediaService = mediaService;
         _configuration = configuration;
      }

      public async Task ResetToSampleData(SampleDataOption model)
      {
         //var sampleFolder = Path.Combine(
         //   GlobalVariables.ContentRootPath,
         //   "Sample",
         //   model.Industry
         //);

         var sampleFolder = Path.Combine(
            GlobalVariables.ContentRootPath,
            _configuration["SampleFolder"],
            model.Industry
         );

         var filePath = Path.Combine(sampleFolder, "ResetToSampleData.sql");

         var lines = File.ReadLines(filePath);

         var commands = _sqlRepository.ParseCommand(lines);

         _sqlRepository.RunCommands(commands);

         //await CopyImages(sampleFolder);
      }

      private async Task CopyImages(string sampleFolder)
      {
         var imagesFolder = Path.Combine(sampleFolder, "Images");
         IEnumerable<string> files = Directory.GetFiles(imagesFolder);
         foreach (var file in files)
         {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
               await _mediaService.SaveMediaAsync(stream, Path.GetFileName(file));
            }
         }
      }
   }
}
