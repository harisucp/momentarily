namespace Apeek.Entities.Web
{
    public interface IApeekPrincipal
    {
        int UserId { get; set; }
        string EmailAddress { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string UserIconUrl { get; set; }
        string[] roles { get; set; }
        int Version { get; set; }
        int UnreadMessageCount { get; set; }
        bool IsUserConfirmed { get; set; }
        bool IsAdmin { get; set; }
         bool IsBlocked { get; set; }
        bool IsMobileVerified { get; set; }
    }
}