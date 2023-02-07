using Apeek.Core.Services;
using Apeek.Entities.Entities;
using DotNetOpenAuth.AspNet;
namespace Apeek.Web.Framework.Auth.AuthControllers
{
    public interface IAuthController
    {
        User Authenticate(AuthenticationResult authenticationResult, IAccountDataService userDataService, int? userId);
        AuthenticationResult CreateAuthenticationResult(bool isSuccessful, string provider, string providerUserId,
            string userName, string email, string imageUrl);
    }
}
