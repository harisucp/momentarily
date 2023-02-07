using System;
using Momentarily.Cron;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Momentarily.UI.Service.Services;
using Apeek.Common;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Logger;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services;
using Momentarily.UI.Service.Services.Impl;
namespace Momentarily.EventManager.TaskExecutor
{
    public class SendReviewEmailTaskExecutor : EventManagerTaskExecutor
    {
        public SendReviewEmailTaskExecutor(ICronObject cronobject, int taskId)
            :base(cronobject, taskId)
        {
        }
        private void ExecuteEmailSend(ICronObject cronobject)
        {
            //var _momentarilyRequestService = Ioc.Get<IMomentarilyGoodRequestService>();
            //var _emailService = Ioc.Get<ISendMessageService>();
            //var _messageService = Ioc.Get<IMomentarilyUserMessageService>();
            //var url = new UrlHelper(HttpContextFactory.Current.Request.RequestContext);
            //var _urlGenerator = Ioc.Get<IUrlGenerator>();
            //var quickUrl = new QuickUrl(new MvcUrlHelper(url), _urlGenerator);
            //var reviewBooking = _momentarilyRequestService.GetBookingForReview();
            //if (reviewBooking.CreateResult == CreateResult.Success)
            //{
            //    foreach (var p in reviewBooking.Obj)
            //    {
            //        if (cronobject.IsStopRequested)
            //        {
            //            return;
            //        }
            //        _messageService.SendReviewMessageForSeeker(p.SharerId, p.SeekerId, p.GoodRequestId, quickUrl);
            //        _messageService.SendReviewMessageForSharer(p.SeekerId, p.SharerId, p.GoodRequestId, quickUrl);
            //        var bool1= _emailService.SendReviewMessageForSeeker(p.SeekerEmail,p.SharerFullName,  p.GoodName,quickUrl.ReviewSeekerAbsoluteUrl(p.GoodRequestId));
            //        var bool2 = _emailService.SendReviewMessageForSharer(p.SharerEmail,p.SeekerFullName, p.GoodName, quickUrl.ReviewSharerAbsoluteUrl(p.GoodRequestId));
            //        if (bool1 || bool2)
            //        {
            //            _momentarilyRequestService.ReviewingUserRequest(p.SeekerId, p.GoodRequestId);
            //        }
            //    }
            //}
        }
        public override void Execute()
        {
           // ExecuteEmailSend(_cronobject);
        }
    }
}
