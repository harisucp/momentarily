using Apeek.Web.Framework.Auth.AuthClients;
namespace Apeek.Web.Framework.Auth.AuthControllers
{
    public class AuthControllerFactory
    {
        public static IAuthController CreateAuthcontroller(string providerName)
        {
            switch (providerName)
            {
                case AuthClient.GooglePlus:
                    return new GooglePlusAuthController();
                case AuthClient.Facebook:
                    return new FacebookAuthController();
                case AuthClient.Vkontakte:
                    return new VkAuthController();
                default: return null;
            }
        }
    }
}