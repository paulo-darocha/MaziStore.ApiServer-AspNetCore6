using MaziStore.Module.Catalog.Models;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Services
{
   public interface IBrandService
   {
      Task Create(Brand brand);

      Task Update(Brand brand);

      Task Delete(long id);

      Task Delete(Brand brand);
   }
}
