using System.Globalization;

namespace MaziStore.Module.Core.Services
{
   public interface ICurrencyService
   {
      CultureInfo CurrencyCulture { get; }

      string FormatCurrency(decimal value);
   }
}
