using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure;
using MaziStore.Module.Orders.Areas.Orders.ViewModels;
using MaziStore.Module.Orders.Models;
using System.Threading.Tasks;

namespace MaziStore.Module.Orders.Services
{
	public interface IOrderService
   {
      Task<MaziResult<Order>> CreateOrder(
         long cartId,
         string paymentMethod,
         decimal paymentFeeAmount,
         OrderStatus orderStatus = OrderStatus.New
      );

      Task<MaziResult<Order>> CreateOrder(
         long cartId,
         string paymentMethod,
         decimal paymentFeeAmount,
         string shippingMethod,
         Address billingAddress,
         Address shippingAddress,
         OrderStatus orderStatus = OrderStatus.New
      );

      void CancelOrder(Order order);

      Task<decimal> GetTax(
         long cartId,
         string countryId,
         long stateOrProvinceId,
         string zipCode
      );

      Task<OrderTaxAndShippingPriceVm> UpdateTaxAndShippingPrices(
         long cartId,
         TaxAndShippingPriceRequestVm model
      );
   }
}
