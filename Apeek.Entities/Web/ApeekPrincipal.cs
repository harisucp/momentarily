using System;
using System.Linq;
using System.Security.Principal;
using Apeek.Entities.Validators;
namespace Apeek.Entities.Web
{
    [Serializable]
    public class ApeekPrincipal : IApeekPrincipal, IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public ApeekPrincipal(IApeekPrincipal apeekPrincipal)
        {
            string identityName = string.Empty;
            if (ValidatorBase.Identity(apeekPrincipal.UserId))
                identityName = apeekPrincipal.UserId.ToString();
            Identity = new GenericIdentity(identityName);
            UserId = apeekPrincipal.UserId;
            EmailAddress = apeekPrincipal.EmailAddress;
            FirstName = apeekPrincipal.FirstName;
            LastName = apeekPrincipal.LastName;
            UserIconUrl = apeekPrincipal.UserIconUrl;
            Version = apeekPrincipal.Version;
            UnreadMessageCount = apeekPrincipal.UnreadMessageCount;
            IsUserConfirmed = apeekPrincipal.IsUserConfirmed;
            IsAdmin = apeekPrincipal.IsAdmin;
            IsBlocked = apeekPrincipal.IsBlocked;
            IsMobileVerified = apeekPrincipal.IsMobileVerified;
            roles = new string[0];
        }
        public ApeekPrincipal(int userId, string userEmail) : this(new ApeekPrincipalSerializeModel() { UserId = userId, EmailAddress = userEmail }) { }
        public bool IsInRole(string role)
        {
            if (roles.Any(r => role.Contains(r)))
            {
                return true;
            }
            return false;
        }
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserIconUrl { get; set; }
        public string SubDomainUrl { get; set; }
        public string LocationUrl { get; set; }
        public int? LocationId { get; set; }
        public string[] roles { get; set; }
        public int Version { get; set; }
        public int UnreadMessageCount { get; set; }
        public bool IsUserConfirmed { get; set; }
        public bool IsAdmin { get; set; }        
        public bool IsBlocked { get; set; }
        public bool IsMobileVerified { get; set; }
    }
}