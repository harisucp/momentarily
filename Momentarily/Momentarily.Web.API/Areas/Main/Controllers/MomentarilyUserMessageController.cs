using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Web.API.Areas.Main.Controllers;
using Momentarily.UI.Service.Services;
namespace Momentarily.Web.API.Areas.Main.Controllers
{
    public class MomentarilyUserMessageController : UserMessageController
    {
        public MomentarilyUserMessageController(IMomentarilyUserMessageService momentarilyUserMessageService,
            IMomentarilyUserNotificationService momentarilyUserNotificationService, ISendMessageService sendMessageService,
            IMomentarilyAccountDataService momentarilyAccountDataService)
            : base(momentarilyUserMessageService, momentarilyUserNotificationService, sendMessageService, momentarilyAccountDataService)
        {
        }
    }
}