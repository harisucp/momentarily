using Apeek.Common;
using Apeek.Entities.Entities;
using Apeek.Entities.Web;
namespace Apeek.Test.Common.Utils
{
    public class TestAuth
    {
        public static void AuthenticateAsAdmin(User model)
        {
            var principle = new ApeekPrincipal(model.Id, null);
            principle.roles = new[] { Privileges.CanEditUsers };
            BaseTest.SetAuthenticatedContext(principle);
        }
        public static void AuthenticateAsUser(User model)
        {
            var principle = new ApeekPrincipal(model.Id, null);
            BaseTest.SetAuthenticatedContext(principle);
        }
        public static void AuthenticateAsUser()
        {
            var principle = new ApeekPrincipal(1, null);
            BaseTest.SetAuthenticatedContext(principle);
        }
        public static void AuthenticateAsNoUser()
        {
            BaseTest.SetAuthenticatedContext(null);
        }
    }
}