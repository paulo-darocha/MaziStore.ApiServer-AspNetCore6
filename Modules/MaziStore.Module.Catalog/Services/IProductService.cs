using MaziStore.Module.Catalog.Models;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Services
{
   public interface IProductService
   {
      void Create(Product product);

      void Update(Product product);

      Task Delete(Product product);
   }
}
