namespace BraintreePayments.Models
{
    public class MerchantAccountIndividual
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string StreetAddress { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public MerchantAccountIndividual()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Phone = string.Empty;
            DateOfBirth = string.Empty;
            Email = string.Empty;
            StreetAddress = string.Empty;
            Locality = string.Empty;
            Region = string.Empty;
            PostalCode = string.Empty;
        }
    }
}
