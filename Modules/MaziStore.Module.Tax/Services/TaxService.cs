using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Tax.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.Tax.Services
{
   public class TaxService : ITaxService
   {
      private readonly IRepository<TaxRate> _taxRateRepository;

      public TaxService(IRepository<TaxRate> taxRateRepository)
      {
         _taxRateRepository = taxRateRepository;
      }

      public async Task<decimal> GetTaxPercent(
         long? taxClassId,
         string countryId,
         long stateOrProvinceId,
         string zipCode
      )
      {
         if (!taxClassId.HasValue)
         {
            return 0;
         }

         var query = _taxRateRepository
            .QueryRp()
            .Where(
               x =>
                  x.CountryId == countryId
                  && (
                     x.StateOrProvinceId == stateOrProvinceId
                     || x.StateOrProvinceId == null
                  )
                  && x.TaxClassId == taxClassId.Value
            );

         if (!string.IsNullOrEmpty(zipCode))
         {
            query = query.Where(
               x => x.ZipCode == zipCode || string.IsNullOrWhiteSpace(x.ZipCode)
            );
         }

         var taxRate = await query.FirstOrDefaultAsync();
         if (taxRate != null)
         {
            return taxRate.Rate;
         }

         return 0;
      }
   }
}
