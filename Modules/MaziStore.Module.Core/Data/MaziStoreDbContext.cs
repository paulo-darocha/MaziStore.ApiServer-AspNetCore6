using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MaziStore.Module.Core.Data
{
   public class MaziStoreDbContext
      : IdentityDbContext<
         User,
         Role,
         long,
         IdentityUserClaim<long>,
         UserRole,
         IdentityUserLogin<long>,
         IdentityRoleClaim<long>,
         IdentityUserToken<long>
      >
   {
      public MaziStoreDbContext(DbContextOptions options)
         : base(options) { }

      public override int SaveChanges(bool acceptAllChangesOnSuccess)
      {
         ValidateEntities();
         return base.SaveChanges(acceptAllChangesOnSuccess);
      }

      public override Task<int> SaveChangesAsync(
         bool acceptAllChangesOnSuccess,
         CancellationToken cancellationToken = default
      )
      {
         ValidateEntities();
         return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         List<Type> typesToRegister = new List<Type>();
         foreach (var module in GlobalVariables.Modules)
         {
            typesToRegister.AddRange(
               module.Assembly.DefinedTypes.Select(x => x.AsType())
            );
         }

         RegisterEntities(modelBuilder, typesToRegister);

         RegisterConventions(modelBuilder);

         base.OnModelCreating(modelBuilder);

         RegisterCustomMapping(modelBuilder, typesToRegister);

         foreach (
            var property in modelBuilder.Model
               .GetEntityTypes()
               .SelectMany(x => x.GetProperties())
               .Where(
                  y => y.ClrType == typeof(decimal) || y.ClrType == typeof(decimal?)
               )
         )
         {
            property.SetColumnType("decimal(18,2)");
         }
      }

      // ///////////////////////////////////////

      private void ValidateEntities()
      {
         var modifiedEntities = ChangeTracker
            .Entries()
            .Where(
               x =>
                  (x.State == EntityState.Added || x.State == EntityState.Modified)
            );

         foreach (var entry in modifiedEntities)
         {
            if (entry.Entity is ValidatableObject validatableObject)
            {
               var validationResults = validatableObject.Validate();
               if (validationResults.Any())
               {
                  throw new ValidationException(
                     validationResults,
                     entry.Entity.GetType()
                  );
               }
            }
         }
      }

      private static void RegisterEntities(
         ModelBuilder modelBuilder,
         IEnumerable<Type> typesToRegister
      )
      {
         var entityTypes = typesToRegister.Where(
            x =>
               x.GetTypeInfo().IsSubclassOf(typeof(EntityBase))
               && !x.GetTypeInfo().IsAbstract
         );

         foreach (var type in entityTypes)
         {
            modelBuilder.Entity(type);
         }
      }

      private static void RegisterConventions(ModelBuilder modelBuilder)
      {
         foreach (var entityType in modelBuilder.Model.GetEntityTypes())
         {
            if (entityType.ClrType.Name != null)
            {
               var nameParts = entityType.ClrType.Namespace.Split('.');
               var tableName = string.Concat(
                  nameParts[2],
                  "_",
                  entityType.ClrType.Name
               );
               modelBuilder.Entity(entityType.Name).ToTable(tableName);
            }
         }

         foreach (
            var relationship in modelBuilder.Model
               .GetEntityTypes()
               .SelectMany(x => x.GetForeignKeys())
         )
         {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
         }
      }

      private static void RegisterCustomMapping(
         ModelBuilder modelBuilder,
         IEnumerable<Type> typesToRegister
      )
      {
         var customModelBuilderTypes = typesToRegister.Where(
            x => typeof(ICustomModelBuilder).IsAssignableFrom(x)
         );

         foreach (var builderType in customModelBuilderTypes)
         {
            if (builderType != null && builderType != typeof(ICustomModelBuilder))
            {
               var builder = (ICustomModelBuilder)
                  Activator.CreateInstance(builderType);
               builder.Build(modelBuilder);
            }
         }
      }
   }
}
