using Apeek.Web.API.Areas.Main.Controllers;
using Momentarily.UI.Service.Services;
namespace Momentarily.Web.API.Areas.Main.Controllers
{
    public class MomentarilyUserNotificationController : UserNotificationController
    {
        public MomentarilyUserNotificationController(IMomentarilyUserNotificationService momentarilyUserNotificationService)
            : base(momentarilyUserNotificationService)
        {
        }
    }
}