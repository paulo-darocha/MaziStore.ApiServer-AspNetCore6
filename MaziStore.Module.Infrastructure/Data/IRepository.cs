using MaziStore.Module.Infrastructure.Models;

namespace MaziStore.Module.Infrastructure.Data
{
   public interface IRepository<T> : IRepositoryWithTypedId<T, long>
      where T : IEntityWithTypedId<long> { }
}
