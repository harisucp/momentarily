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
    public class PayoutsTaskExecutor : EventManagerTaskExecutor
    {
        public PayoutsTaskExecutor(ICronObject cronobject, int taskId)
            :base(cronobject, taskId)
        {
        }
        private void ExecutePayout(ICronObject cronobject)
        {
            var _momentarilyRequestService = Ioc.Get<IMomentarilyGoodRequestService>();
            var _paymentService = new PinPaymentService();
            var payouts = _momentarilyRequestService.GetGoodRequestForPayout();
            if (payouts.CreateResult == CreateResult.Success)
            {
                var payoutWithToken = payouts.Obj.Where(p => !String.IsNullOrEmpty(p.RecipientToken)).ToList();
                var payoutWithoutToken = payouts.Obj.Where(p => String.IsNullOrEmpty(p.RecipientToken)).ToList();
                foreach (var p in payoutWithToken)
                {
                    if (cronobject.IsStopRequested)
                    {
                        return;
                    }
                    Ioc.Get<IDbLogger>().LogMessage(LogSource.PinPaymentService, String.Format("Try to payout for goodd request id: {0} for user Id: {1}",p.GoodRequestId, p.UserId));
                    var result = _paymentService.Payout(p);
                }
            }
        }
        public override void Execute()
        {
            ExecutePayout(_cronobject);
        }
    }
}
