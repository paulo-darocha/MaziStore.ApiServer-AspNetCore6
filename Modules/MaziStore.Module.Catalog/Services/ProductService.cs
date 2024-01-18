using MaziStore.Module.Catalog.Models;
using MaziStore.Module.Core.Services;
using MaziStore.Module.Infrastructure.Data;
using System.Threading.Tasks;

namespace MaziStore.Module.Catalog.Services
{
   public class ProductService : IProductService
   {
      private const string ProductEntityTypeId = "Product";
      private readonly IRepository<Product> _productRepository;
      private readonly IEntityService _entityService;

      public ProductService(
         IRepository<Product> productRepository,
         IEntityService entityService
      )
      {
         _productRepository = productRepository;
         _entityService = entityService;
      }

      public void Create(Product product)
      {
         using (var transaction = _productRepository.BeginTransactionRp())
         {
            product.Slug = _entityService.ToSafeSlug(
               product.Slug,
               product.Id,
               ProductEntityTypeId
            );
            _productRepository.AddRp(product);
            _productRepository.SaveChangesRp();

            _entityService.Add(
               product.Name,
               product.Slug,
               product.Id,
               ProductEntityTypeId
            );
            _productRepository.SaveChangesRp();

            transaction.Commit();
         }
      }

      public void Update(Product product)
      {
         var slug = _entityService.Get(product.Id, ProductEntityTypeId);
         if (product.IsVisibleIndividually)
         {
            product.Slug = _entityService.ToSafeSlug(
               product.Slug,
               product.Id,
               ProductEntityTypeId
            );
            if (slug != null)
            {
               _entityService.Update(
                  product.Name,
                  product.Slug,
                  product.Id,
                  ProductEntityTypeId
               );
            }
            else
            {
               _entityService.Add(
                  product.Name,
                  product.Slug,
                  product.Id,
                  ProductEntityTypeId
               );
            }
         }
         else
         {
            if (slug != null)
            {
               _entityService.Remove(product.Id, ProductEntityTypeId);
            }
         }

         _productRepository.SaveChangesRp();
      }

      public async Task Delete(Product product)
      {
         product.IsDeleted = true;
         await _entityService.Remove(product.Id, ProductEntityTypeId);
         _productRepository.SaveChangesRp();
      }
   }
}
