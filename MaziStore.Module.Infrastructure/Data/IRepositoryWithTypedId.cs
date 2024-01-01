using MaziStore.Module.Infrastructure.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace MaziStore.Module.Infrastructure.Data
{
   public interface IRepositoryWithTypedId<T, TId>
      where T : IEntityWithTypedId<TId>
   {
      IQueryable<T> QueryRp();

      void AddRp(T entity);

      void AddRangeRp(IEnumerable<T> entities);

      IDbContextTransaction BeginTransactionRp();

      void SaveChangesRp();

      void SaveChangesRpAsync();

      void RemoveRp(T entity);
   }
}
