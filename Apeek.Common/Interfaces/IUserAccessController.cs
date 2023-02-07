using Apeek.Entities.Web;
namespace Apeek.Common.Interfaces
{
    public interface IUserAccessController : IDependency
    {
        int? UserId { get; }
        /// <summary>
        /// If the editing user is null we check if user has privilege to accessToUserID
        /// If the editing user is authenticated user access will true. 
        /// Else we check if user has privilege to accessToUserID
        /// </summary>
        /// <param name="privilege">Privilege name</param>
        /// <param name="accessToUserID">Authenticated user or user from query string</param>
        bool HasAccess(string privilege, int? accessToUserID = null);
        bool AuthenticateUser(int personId);
        void SignOutUser();
        void UpdateUser(string newEmail = null, string firstName = null, string lastName = null, string userIconUrl = null);
        void UpdateUser(ApeekPrincipalSerializeModel serializeModel);
        void PostAuthRequest();
        ApeekPrincipalSerializeModel UpdateCookie();
    }
}