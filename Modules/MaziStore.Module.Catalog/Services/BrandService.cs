using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Services
{
   public class BrandService : IBrandService
   {
      private const string BrandEntityTypeId = "Brand";
      private readonly IRepository<Brand> _brandRepository;
      private readonly IEntityService _entityService;

      public BrandService(
         IRepository<Brand> brandRepository,
         IEntityService entityService
      )
      {
         _brandRepository = brandRepository;
         _entityService = entityService;
      }

      public async Task Create(Brand brand)
      {
         using (var transaction = _brandRepository.BeginTransactionRp())
         {
            brand.Slug = _entityService.ToSafeSlug(
               brand.Slug,
               brand.Id,
               BrandEntityTypeId
            );
            _brandRepository.AddRp(brand);
            await _brandRepository.SaveChangesRpAsync();

            _entityService.Add(brand.Name, brand.Slug, brand.Id, BrandEntityTypeId);
            await _brandRepository.SaveChangesRpAsync();

            transaction.Commit();
         }
      }

      public async Task Update(Brand brand)
      {
         brand.Slug = _entityService.ToSafeSlug(
            brand.Slug,
            brand.Id,
            BrandEntityTypeId
         );
         _entityService.Update(brand.Name, brand.Slug, brand.Id, BrandEntityTypeId);
         await _brandRepository.SaveChangesRpAsync();
      }

      public async Task Delete(long id)
      {
         var brand = _brandRepository.QueryRp().First(x => x.Id == id);
         await Delete(brand);
      }

      public async Task Delete(Brand brand)
      {
         brand.IsDeleted = true;
         await _entityService.Remove(brand.Id, BrandEntityTypeId);
         _brandRepository.SaveChangesRp();
      }
   }
}
