namespace Apeek.Web.Framework.Auth.AuthClients
{
    public interface IExternalAuthClient
    {
        bool UserExists(string userId, string accessToken);
    }
}
