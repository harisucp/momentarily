using Apeek.Web.Framework.Auth.AuthClients;
using Microsoft.Web.WebPages.OAuth;
namespace Momentarily.Web
{
    public class AuthConfig
    {
        public static void RegisterAuth()
        {
            OAuthWebSecurity.RegisterClient(AuthClientFactory.CreateAuthClient(AuthClient.GooglePlus), AuthClient.GooglePlus, null);
            OAuthWebSecurity.RegisterClient(AuthClientFactory.CreateAuthClient(AuthClient.Facebook), AuthClient.Facebook, null);
        }
    }
}