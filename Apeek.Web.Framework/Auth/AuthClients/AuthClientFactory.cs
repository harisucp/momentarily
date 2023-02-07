using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using DotNetOpenAuth.AspNet;
namespace Apeek.Web.Framework.Auth.AuthClients
{
    public class AuthClientFactory
    {
        private static ISettingsDataService _settings;
        static AuthClientFactory()
        {
            _settings = new SettingsDataService();
        }
        public static VkClient CreateVkClient()
        {
            //http://vk.com/editapp?id=4504381&section=options
            return new VkClient(_settings.GetVkClientId(), _settings.GetVkClientSecret());
        }
        public static FacebookClient CreateFacebookClient()
        {
            //https://developers.facebook.com/apps/1542099459344893/dashboard/
            return new FacebookClient(_settings.GetFacebookClientId(), _settings.GetFacebookClientSecret()); 
        }
        public static GooglePlusClient CreateGooglePlusClient()
        {
            //https://console.developers.google.com/project
            return new GooglePlusClient(_settings.GetGoogleClientId(), _settings.GetGoogleClientSecret());
        }
        public static IAuthenticationClient CreateAuthClient(string providerName)
        {
            switch (providerName)
            {
                case AuthClient.Facebook:
                    return CreateFacebookClient();
                case AuthClient.Vkontakte:
                    return CreateVkClient();
                case AuthClient.GooglePlus:
                    return CreateGooglePlusClient();
                default:
                    Ioc.Get<IDbLogger>().LogError(LogSource.AuthController, "Provider Name [{0}] is not defined while creating auth client cannot find", providerName);
                    return null;
            }
        }
    }
}