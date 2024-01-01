using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.News.Models;
using Microsoft.EntityFrameworkCore;

namespace MaziStore.Module.News.Data
{
   public class NewsCustomModelBuilder : ICustomModelBuilder
   {
      public void Build(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<NewsItemCategory>(newsItemCat =>
         {
            newsItemCat.HasKey(x => new { x.CategoryId, x.NewsItemId });
            newsItemCat
               .HasOne(x => x.Category)
               .WithMany(y => y.NewsItems)
               .HasForeignKey(x => x.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);
            newsItemCat
               .HasOne(x => x.NewsItem)
               .WithMany(y => y.Categories)
               .HasForeignKey(x => x.NewsItemId)
               .OnDelete(DeleteBehavior.Cascade);
            newsItemCat.ToTable("News_NewsItemCatgory");
         });

         modelBuilder
            .Entity<AppSetting>()
            .HasData(
               new AppSetting("News.PageSize")
               {
                  Module = "News",
                  IsVisibleInCommonSettingPage = true,
                  Value = "10"
               }
            );

         modelBuilder
            .Entity<EntityType>()
            .HasData(
               new EntityType("NewsCategory")
               {
                  AreaName = "News",
                  RoutingController = "NewsCategory",
                  RoutingAction = "NewsCategoryDetail",
                  IsMenuable = true
               },
               new EntityType("NewsItem")
               {
                  AreaName = "News",
                  RoutingController = "NewsItem",
                  RoutingAction = "NewsItemDetail",
                  IsMenuable = false
               }
            );
      }
   }
}
