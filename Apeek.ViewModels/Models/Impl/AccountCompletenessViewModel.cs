namespace Apeek.ViewModels.Models
{
    public class AccountCompletenessViewModel
    {
        public bool HasServices { get; set; }
        public bool HasDescription { get; set; }
        public bool HasFullName { get; set; }
        public bool HasPwd { get; set; }
        public bool IsVerified { get; set; }
    }
}