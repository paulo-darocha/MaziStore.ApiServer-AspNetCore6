using MaziStore.Module.Core.Models;
using System.Linq;

namespace MaziStore.Module.Core.Services
{
   public interface IWidgetInstanceService
   {
      IQueryable<WidgetInstance> GetPublished();
   }
}
