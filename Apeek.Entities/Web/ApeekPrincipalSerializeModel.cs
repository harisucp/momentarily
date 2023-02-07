using System;
using System.Linq;
namespace Apeek.Entities.Web
{
    public class ApeekPrincipalSerializeModel : IApeekPrincipal
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserIconUrl { get; set; }
        public string[] roles { get; set; }
        public int Version { get; set; }
        public int UnreadMessageCount { get;set;}
        public bool IsUserConfirmed { get; set; }
        public bool IsAdmin { get; set; }        
        public bool IsBlocked { get; set; }
        public bool IsMobileVerified { get; set; }
        public ApeekPrincipalSerializeModel(IApeekPrincipal tophandsPrincipal)
        {
            UserId = tophandsPrincipal.UserId;
            EmailAddress = tophandsPrincipal.EmailAddress;
            if (tophandsPrincipal.roles.Any())
            {
                roles = new string[tophandsPrincipal.roles.Length];
                Array.Copy(tophandsPrincipal.roles, roles, tophandsPrincipal.roles.Length);
            }
            Version = tophandsPrincipal.Version;
        }
        public ApeekPrincipalSerializeModel()
        {
        }
    }
}