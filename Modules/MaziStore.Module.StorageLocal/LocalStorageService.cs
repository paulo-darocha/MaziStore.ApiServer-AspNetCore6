using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure;
using System.IO;
using System.Threading.Tasks;

namespace MaziStore.Module.StorageLocal
{
   public class LocalStorageService : IStorageService
   {
      private const string MediaFolder = "images";

      public string GetMediaUrl(string fileName)
      {
         return $"/{MediaFolder}/{fileName}";
      }

      public async Task SaveMediaAsync(
         Stream mediaBinaryStream,
         string fileName,
         string mimeType = null
      )
      {
         var filePath = Path.Combine(
            GlobalVariables.WebRootPath,
            MediaFolder,
            fileName
         );
         using (var output = new FileStream(filePath, FileMode.Create))
         {
            await mediaBinaryStream.CopyToAsync(output);
         }
      }

      public async Task DeleteMediaAsync(string fileName)
      {
         var filePath = Path.Combine(
            GlobalVariables.WebRootPath,
            MediaFolder,
            fileName
         );
         if (File.Exists(fileName))
         {
            await Task.Run(() => File.Delete(filePath));
         }
      }
   }
}
