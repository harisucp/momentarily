using Apeek.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Apeek.Common;
using Apeek.Core.Services;
using Apeek.ViewModels.Models.Impl;
using Momentarily.UI.Service.Services;

namespace Apeek.Web.API.Areas.Main.Controllers
{
    public class UserDataController : BaseApiController<UserDataViewModel, int>
    {
        protected IUserMessageService _userMessageService;
        private readonly IUserNotificationService _userNotificationService;

        public UserDataController(IUserMessageService userMessageService,IUserNotificationService userNotificationService)
        {
            _userMessageService = userMessageService;
            _userNotificationService = userNotificationService;
        }
        //public HttpResponseMessage Get()
        //{
        //    UserDataViewModel model = new UserDataViewModel();
        //    if (UserId.HasValue)
        //    {
        //        var result = _userMessageService.GetUnreadMessageCount(UserId.Value);
        //        model.UnreadMessagesCount = result;
        //        model.Id = UserId.Value;
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, model);
        //}
        public HttpResponseMessage Get()
        {
            UserDataViewModel model = new UserDataViewModel();
            if (UserId.HasValue)
            {
                var result = _userMessageService.GetUnreadMessageCount(UserId.Value);
                var notification = _userNotificationService.GetNotifications(UserId.Value);
                model.UnreadMessagesCount = result;
                model.UserNotification = notification.Obj.Count();
                model.Id = UserId.Value;
            }
            return Request.CreateResponse(HttpStatusCode.OK, model);
        }
    }
}