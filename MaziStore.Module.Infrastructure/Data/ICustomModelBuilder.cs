using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Infrastructure.Data
{
   public interface ICustomModelBuilder
   {
      void Build(ModelBuilder modelBuilder);
   }
}
