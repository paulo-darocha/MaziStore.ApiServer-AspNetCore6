using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace MaziStore.Module.Core.Data
{
   public class RepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId>
      where T : class, IEntityWithTypedId<TId>
   {
      public RepositoryWithTypedId(MaziStoreDbContext context)
      {
         DbContextRp = context;
         DbSetRp = DbContextRp.Set<T>();
      }

      protected DbContext DbContextRp { get; }

      protected DbSet<T> DbSetRp { get; }

      public void AddRangeRp(IEnumerable<T> entities)
      {
         DbSetRp.AddRange(entities);
      }

      public void AddRp(T entity)
      {
         DbSetRp.Add(entity);
      }

      public IDbContextTransaction BeginTransactionRp()
      {
         return DbContextRp.Database.BeginTransaction();
      }

      public IQueryable<T> QueryRp()
      {
         return DbSetRp;
      }

      public void RemoveRp(T entity)
      {
         DbSetRp.Remove(entity);
      }

      public void SaveChangesRp()
      {
         DbContextRp.SaveChanges();
      }

      public void SaveChangesRpAsync()
      {
         DbContextRp.SaveChangesAsync();
      }
   }
}
