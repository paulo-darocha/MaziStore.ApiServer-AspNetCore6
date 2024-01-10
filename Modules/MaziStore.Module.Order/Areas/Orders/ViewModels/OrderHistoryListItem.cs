using MaziStore.Module.Core.Services;
using MaziStore.Module.Orders.Models;
using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;

namespace MaziStore.Module.Orders.Areas.Orders.ViewModels
{
   public class OrderHistoryListItem
   {
      private readonly ICurrencyService _currencyService;

      public OrderHistoryListItem(ICurrencyService currencyService)
      {
         _currencyService = currencyService;
      }

      public long Id { get; set; }

      public DateTimeOffset CreatedOn { get; set; }

      public decimal SubTotal { get; set; }

      public string SubTotalString
      {
         get { return _currencyService.FormatCurrency(SubTotal); }
      }

      [JsonConverter(typeof(JsonStringEnumConverter))]
      public OrderStatus OrderStatus { get; set; }

      public IList<OrderHistoryProductVm> OrderItems { get; set; } =
         new List<OrderHistoryProductVm>();
   }
}
