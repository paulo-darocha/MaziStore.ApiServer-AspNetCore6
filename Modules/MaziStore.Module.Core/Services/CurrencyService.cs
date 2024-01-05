using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace MaziStore.Module.Core.Services
{
   public class CurrencyService : ICurrencyService
   {
      public CurrencyService()
      {
         CurrencyCulture = new CultureInfo("pt-BR");
      }

      public CultureInfo CurrencyCulture { get; }

      public string FormatCurrency(decimal value)
      {
         var decimalPlace = 2;
         return value.ToString($"C{decimalPlace}", CurrencyCulture);
      }
   }
}
