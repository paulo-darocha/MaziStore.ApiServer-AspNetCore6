using MaziStore.Module.Infrastructure.Data;
using MaziStore.Module.Shipping.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaziStore.Module.ShippingPrices.Services
{
   public class ShippingPriceService : IShippingPriceService
   {
      private readonly IServiceProvider _serviceProvider;
      private readonly IRepositoryWithTypedId<
         ShippingProvider,
         string
      > _shippingProviderRepository;

      public ShippingPriceService(
         IServiceProvider serviceProvider,
         IRepositoryWithTypedId<ShippingProvider, string> shippingProviderRepository
      )
      {
         _serviceProvider = serviceProvider;
         _shippingProviderRepository = shippingProviderRepository;
      }

      public async Task<IList<ShippingPrice>> GetApplicableShippingPrices(
         GetShippingPriceRequest request
      )
      {
         var applicableShippingPrices = new List<ShippingPrice>();
         var providers = await _shippingProviderRepository.QueryRp().ToListAsync();
         var shippingRateServices =
            _serviceProvider.GetServices<IShippingPriceServiceProvider>();

         foreach (var provider in providers)
         {
            if (!provider.ToAllShippingEnabledCountries)
            {
               if (
                  !provider.OnlyCountryIds.Contains(
                     request.ShippingAddress.CountryId
                  )
               )
               {
                  continue;
               }
            }

            if (!provider.ToAllShippingEnabledStatesOrProvinces)
            {
               if (
                  !provider.OnlyStateOrProvinceIds.Contains(
                     request.ShippingAddress.StateOrProvinceId
                  )
               )
               {
                  continue;
               }
            }

            var priceServiceType = Type.GetType(
               provider.ShippingPriceServiceTypeName
            );
            var priceService = shippingRateServices
               .Where(x => x.GetType() == priceServiceType)
               .FirstOrDefault();
            var response = await priceService.GetShippingPrices(request, provider);
            applicableShippingPrices.AddRange(response.ApplicablePrices);
         }

         return applicableShippingPrices;
      }
   }
}
