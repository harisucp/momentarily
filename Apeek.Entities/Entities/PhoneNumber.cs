using Apeek.Entities.Validators;
namespace Apeek.Entities.Entities
{
    public class PhoneNumber
    {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual Address Address { get; set; }
        public virtual string CountryCode { get; set; }
        [ValidatorPhoneNumber]
        public virtual string PhoneNum { get; set; }
    }
}