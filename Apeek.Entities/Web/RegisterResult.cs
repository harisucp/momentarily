using Apeek.Entities.Entities;
namespace Apeek.Entities.Web
{
    public class RegisterResult
    {
        public User User { get; set; }
        public RegisterStatus RegisterStatus { get; set; }
        public RegisterResult()
        {
            RegisterStatus = RegisterStatus.Fail;
        }
    }
}
