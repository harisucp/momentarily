using Apeek.Common;
using Apeek.Core.Services;
using Apeek.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class NotificationController : FrontendController
    {
        private readonly IUserNotificationService _userNotificationService;

        public NotificationController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            var list = _userNotificationService.GetNotificationsList(UserId.Value);
            var shape = _shapeFactory.BuildShape(null, list.Obj.ToList(), PageName.Listing.ToString());
            return DisplayShape(shape);
        }

        public ActionResult MarkRead(int id)
        {
            var list = _userNotificationService.SetIsViewedNotification(UserId.Value,id);
            return RedirectToAction("Index");
        }

        public ActionResult MarkAllRead()
        {
            var list = _userNotificationService.SetIsViewedForAllNotification(UserId.Value);
            return RedirectToAction("Index");
        }
    }
}