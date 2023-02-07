using Apeek.Entities.Entities;
namespace Apeek.Entities.Web
{
    public class LoginResult
    {
        public User User { get; set; }
        public LoginStatus LoginStatus { get; set; }
        public LoginResult()
        {
            LoginStatus = LoginStatus.Fail;
        }
    }
}