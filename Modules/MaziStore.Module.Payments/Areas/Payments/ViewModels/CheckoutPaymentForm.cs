using MaziStore.Module.Payments.Models;
using System.Collections.Generic;

namespace MaziStore.Module.Payments.Areas.Payments.ViewModels
{
   public class CheckoutPaymentForm
   {
      public IList<PaymentProviderVm> PaymentProviders { get; set; } =
         new List<PaymentProviderVm>();
   }
}
