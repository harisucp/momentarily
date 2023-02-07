namespace Apeek.ViewModels.Models
{
    public class MessageSentModel
    {
        public bool IsEmailMessageSent { get; set; }        public bool IsSmsMessageSent { get; set; }        public bool IsExternal { get; set; }        public bool IsVerified { get; set; }        public bool IsMobileVerified { get; set; }        public string VC { get; set; }        public string PhoneNumber { get; set; }
        public string Email { get; set; }


    }
}