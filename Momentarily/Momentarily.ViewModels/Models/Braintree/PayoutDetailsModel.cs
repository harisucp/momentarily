using System.ComponentModel.DataAnnotations;namespace Momentarily.ViewModels.Models.Braintree{    public class PayoutDetailsModel    {
        //public int Id { get; set; }
        [Required(ErrorMessage = "UserId Required")]        [Display(Name = "userid.")]        public int UserId { get; set; }
        //[Required(ErrorMessage = "AccountNumber Required")]
        public string AccountNumber { get; set; }
        //[Required(ErrorMessage = "RoutingNumber Required")]
        public string RoutingNumber { get; set; }        [Required(ErrorMessage = "Locality Required")]        public string Locality { get; set; }        [Required(ErrorMessage = "PostalCode Required")]        public string PostalCode { get; set; }        [Required(ErrorMessage = "Region Required")]        public string Region { get; set; }        [Required(ErrorMessage = "Address Required")]        public string StreetAddress { get; set; }        [Required(ErrorMessage = "Email Required")]        [EmailAddress(ErrorMessage = "Invalid Email Address.")]        public string PaypalBusinessEmail { get; set; }        [Required(ErrorMessage = " Phone number required.")]
        //[MaxLength(10, ErrorMessage = "Maximum 10 digits allowed")]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }        [Required(ErrorMessage = " Payment Type required.")]        public int PaymentType { get; set; }

        [Required(ErrorMessage = "The country is required.")]
        public int? CountryId { get; set; }        

    }}