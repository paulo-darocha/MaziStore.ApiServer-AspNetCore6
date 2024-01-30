using MaziStore.Module.Catalog.Areas.Catalog.ViewModels;
using MaziStore.Module.Catalog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Services
{
   public interface ICategoryService
   {
      Task<IList<CategoryListItem>> GetAll();

      Task Create(Category category);

      Task Update(Category category);

      Task Delete(Category category);
   }
}
