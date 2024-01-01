using MaziStore.Module.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Payments.Models
{
   public class PaymentProvider : EntityBaseWithTypedId<string>
   {
      public PaymentProvider(string id)
      {
         Id = id;
      }

      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Name { get; set; }

      public bool IsEnabled { get; set; }

      [StringLength(450)]
      public string ConfigureUrl { get; set; }

      [StringLength(450)]
      public string LandingViewComponentName { get; set; }

      public string AdditionalSettings { get; set; }
   }
}
