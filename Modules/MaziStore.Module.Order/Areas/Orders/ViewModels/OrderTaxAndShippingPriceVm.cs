using MaziStore.Module.ShippingPrices.Services;
using MaziStore.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using System.Collections.Generic;

namespace MaziStore.Module.Orders.Areas.Orders.ViewModels
{
	public class OrderTaxAndShippingPriceVm
	{
		public bool IsProductPriceIncludedTax { get; set; }

		public IList<ShippingPrice> ShippingPrices { get; set; }

		public string SelectedShippingMethodName { get; set; }

		public CartVm Cart { get; set; }
	}
}
