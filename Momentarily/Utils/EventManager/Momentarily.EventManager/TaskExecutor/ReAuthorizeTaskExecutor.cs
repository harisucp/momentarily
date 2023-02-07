using System;
using Momentarily.Cron;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Momentarily.UI.Service.Services;
using Apeek.Common;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services;
using Momentarily.UI.Service.Services.Impl;

namespace Momentarily.EventManager.TaskExecutor
{
    public class ReAuthorizeTaskExecutor : EventManagerTaskExecutor
    {

        public ReAuthorizeTaskExecutor(ICronObject cronobject, int taskId)
            :base(cronobject, taskId)
        {
        }

        private void ExecuteReAuth(ICronObject cronobject)
        {
            var _momentarilyRequestService = Ioc.Get<IMomentarilyGoodRequestService>();
            var _paymentService = new PinPaymentService();
            var list = _paymentService.GetAuthorizeExpireChargesByDate(DateTime.Now);
            if (list.CreateResult == CreateResult.Success)
            {
                foreach (var c in list.Obj)
                {
                    if (cronobject.IsStopRequested)
                    {
                        return;
                    }
                    _paymentService.ReAuthorize(c);
                }
            }


            //if (payouts.CreateResult == CreateResult.Success)
            //{
            //    var payoutWithToken = payouts.Obj.Where(p => !String.IsNullOrEmpty(p.RecipientToken)).ToList();
            //    var payoutWithoutToken = payouts.Obj.Where(p => String.IsNullOrEmpty(p.RecipientToken)).ToList();
            //    foreach (var p in payoutWithToken)
            //    {
            //        if (cronobject.IsStopRequested)
            //        {
            //            return;
            //        }
            //        var result = _paymentService.Payout(p);
            //    }
            //}
            //EmailTaskExecutor executor = new EmailTaskExecutor();
            //executor.Execute(getUsersToComeBack, notifyUser, _cronobject, _emailType, _taskId);
        }

        public override void Execute()
        {
            ExecuteReAuth(_cronobject);
        }
    }
}
