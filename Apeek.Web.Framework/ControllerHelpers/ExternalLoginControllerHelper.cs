using System;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.Web.Framework.Auth.AuthClients;
using Apeek.Web.Framework.Auth.AuthControllers;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;

namespace Apeek.Web.Framework.ControllerHelpers
{
    public class ExternalLoginControllerHelper : BaseControllerHelper
    {
        public AuthenticationResult VerifyAuthentication(string redirectTo, string callbackUrl)
        {
            Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "Trying to authenticate via external login");
            

            GooglePlusClient.RewriteRequest();

            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(callbackUrl);

            if (!result.IsSuccessful)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.Account, string.Format("Error when external login:{0}", result.Error));
            }
            return result;
        }

        public User AuthenticateUser(AuthenticationResult result)
        {
            var authController = AuthControllerFactory.CreateAuthcontroller(result.Provider);
            if (authController == null) return null;

            try
            {
                var user = authController.Authenticate(result, AccountDataService, UserId);
                if (user != null)
                {
                    return user;
                }

                Ioc.Get<IDbLogger>().LogError(LogSource.Account, "Cannot authenticate user via external login: {0}", authController.ToString());
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogException(LogSource.Account, ex);
                return null;
            }
            return null;
        }
    }
}
