using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.Core.Data
{
   public class CoreCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<AppSetting>().ToTable("Core_AppSetting");

         modelBuilder.Entity<User>().ToTable("Core_User");

         modelBuilder.Entity<Role>().ToTable("Core_Role");

         modelBuilder.Entity<IdentityUserClaim<long>>(idUserClaim =>
         {
            idUserClaim.HasKey(x => x.Id);
            idUserClaim.ToTable("Core_UserClaim");
         });

         modelBuilder.Entity<IdentityRoleClaim<long>>(idRoleClaim =>
         {
            idRoleClaim.HasKey(x => x.Id);
            idRoleClaim.ToTable("Core_RoleClaim");
         });

         modelBuilder.Entity<UserRole>(userRole =>
         {
            userRole.HasKey(x => new { x.UserId, x.RoleId });
            userRole
               .HasOne(x => x.Role)
               .WithMany(y => y.Users)
               .HasForeignKey(x => x.RoleId);
            userRole
               .HasOne(x => x.User)
               .WithMany(y => y.Roles)
               .HasForeignKey(x => x.UserId);
            userRole.ToTable("Core_UserRole");
         });

         modelBuilder.Entity<IdentityUserLogin<long>>(idUserLogin =>
         {
            idUserLogin.ToTable("Core_UserLogin");
         });

         modelBuilder.Entity<IdentityUserToken<long>>(idUserToken =>
         {
            idUserToken.ToTable("Core_UserToken");
         });

         modelBuilder.Entity<Entity>(entity =>
         {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EntityId);
         });

         modelBuilder.Entity<User>(user =>
         {
            user.HasOne(x => x.DefaultShippingAddress)
               .WithMany()
               .HasForeignKey(x => x.DefaultShippingAddressId)
               .OnDelete(DeleteBehavior.Restrict);
            user.HasOne(x => x.DefaultBillingAddress)
               .WithMany()
               .HasForeignKey(x => x.DefaultBillingAddressId)
               .OnDelete(DeleteBehavior.Restrict);
         });

         modelBuilder
            .Entity<UserAddress>()
            .HasOne(x => x.User)
            .WithMany(y => y.UserAddresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

         modelBuilder.Entity<Address>(address =>
         {
            address
               .HasOne(x => x.District)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            address
               .HasOne(x => x.StateOrProvince)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            address
               .HasOne(x => x.Country)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);
         });

         modelBuilder.Entity<CustomerGroup>().HasIndex(x => x.Name).IsUnique();

         modelBuilder.Entity<CustomerGroupUser>(custGroupUser =>
         {
            custGroupUser.HasKey(x => new { x.UserId, x.CustomerGroupId });
            custGroupUser
               .HasOne(x => x.User)
               .WithMany(y => y.CustomerGroups)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
            custGroupUser
               .HasOne(x => x.CustomerGroup)
               .WithMany(y => y.Users)
               .HasForeignKey(x => x.CustomerGroupId)
               .OnDelete(DeleteBehavior.Cascade);
            custGroupUser.ToTable("Core_CustomerGroupUser");
         });

         CoreSeedData.SeedData(modelBuilder);
      }
   }
}
