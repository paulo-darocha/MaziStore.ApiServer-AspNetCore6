using MaziStore.Module.Core.Models;
using MaziStore.Module.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace MaziStore.Module.Inventory.Models
{
   public class Warehouse : EntityBase
   {
      public Warehouse() { }

      public Warehouse(long id)
      {
         Id = id;
      }

      [Required(ErrorMessage = "The {0} field is required.")]
      [StringLength(450)]
      public string Name { get; set; }

      public long AddressId { get; set; }

      public long? VendorId { get; set; }

      public Vendor Vendor { get; set; }

      public Address Address { get; set; }
   }
}
