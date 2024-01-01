using MaziStore.Module.SampleData.Areas.SampleData.ViewModels;
using System.Threading.Tasks;

namespace MaziStore.Module.SampleData.Services
{
   public interface ISampleDataService
   {
      Task ResetToSampleData(SampleDataOption model);
   }
}
