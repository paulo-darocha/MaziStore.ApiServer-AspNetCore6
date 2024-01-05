using MaziStore.Module.ActivityLog.Models;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.ActivityLog.Data
{
   public class ActivityLogCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<Activity>().HasIndex(x => x.ActivityTypeId);

         modelBuilder
            .Entity<ActivityType>()
            .HasData(new ActivityType(1) { Name = "EntityView" });
      }
   }
}
