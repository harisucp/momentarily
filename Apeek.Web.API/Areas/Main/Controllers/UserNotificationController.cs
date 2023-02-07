using System.Net;
using System.Net.Http;
using System.Web.Http;
using Apeek.Common;
using Apeek.Core.Services;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.Infrastructure;
namespace Apeek.Web.API.Areas.Main.Controllers
{
    [Authorize]
    public class UserNotificationController : BaseApiController<UserNotificationViewModel, int>
    {
        private readonly IUserNotificationService _userNotificationService;
        public UserNotificationController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }
        public HttpResponseMessage Get()
        {
            var result = _userNotificationService.GetNotifications(UserId.Value);
            if (result.CreateResult == CreateResult.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result.Obj);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        public HttpResponseMessage Put(int notificationId)
        {
            if (_userNotificationService.SetIsViewedNotification(UserId.Value, notificationId))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}