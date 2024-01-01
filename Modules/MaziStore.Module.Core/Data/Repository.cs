using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Infrastructure.Models;

namespace MaziStore.Module.Core.Data
{
   public class Repository<T> : RepositoryWithTypedId<T, long>, IRepository<T>
      where T : class, IEntityWithTypedId<long>
   {
      public Repository(MaziStoreDbContext context)
         : base(context) { }
   }
}
