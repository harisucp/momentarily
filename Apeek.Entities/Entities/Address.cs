using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Apeek.Entities.Entities
{
    public class Address
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }
        public virtual string Country { get; set; }
        public virtual string Region { get; set; }
        public virtual int? LocationId { get; set; }
        [StringLength(200, ErrorMessage = "AddressLine1 cannot be longer than 200 characters.")]
        public virtual string AddressLine1 { get; set; }
        public virtual string AddressLine2 { get; set; }
        public virtual string AddressLine3 { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual IList<PhoneNumber> PhoneNumberRecords { get; set; }
        public virtual LocationLang LocationLang { get; set; }
        public Address()
        {
            PhoneNumberRecords = new List<PhoneNumber>();
            LocationLang = new LocationLang();
        }
    }
}