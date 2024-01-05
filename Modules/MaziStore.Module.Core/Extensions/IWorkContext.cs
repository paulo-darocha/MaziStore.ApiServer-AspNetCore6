using MaziStore.Module.Core.Models;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Extensions
{
   public interface IWorkContext
   {
      Task<User> GetCurrentUser();
   }
}
