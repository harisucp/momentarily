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

namespace Momentarily.EventManager.TaskExecutor
{
    public class VoidAuthorizationFundsTaskExecutor : EventManagerTaskExecutor
    {

        public VoidAuthorizationFundsTaskExecutor(ICronObject cronobject, int taskId)
            :base(cronobject, taskId)
        {
        }

        private void ExecutePayout(ICronObject cronobject)
        {
            var _momentarilyRequestService = Ioc.Get<IMomentarilyGoodRequestService>();
            var _paymentService = Ioc.Get<IPaymentService>();

            var transactions = _momentarilyRequestService.GetTransactionForVoid();

            if (transactions.CreateResult == CreateResult.Success)
            {
                foreach (var t in transactions.Obj)
                {
                    if (cronobject.IsStopRequested)
                    {
                        return;
                    }
                    var result = _paymentService.VoidAuthorizePayment(t.PaymentId, t.PayerId);
                    if (result)
                    {
                        if (_momentarilyRequestService.CloseGoodRequest(t.GoodsUserId, t.GoodRequestId))
                        {
                            Ioc.Get<IDbLogger>().LogMessage(LogSource.GoodRequestService, string.Format("Close booking RequestId: {0} ", t.GoodRequestId));
                        }
                    }
                }
            }
            //EmailTaskExecutor executor = new EmailTaskExecutor();
            //executor.Execute(getUsersToComeBack, notifyUser, _cronobject, _emailType, _taskId);
        }

        public override void Execute()
        {
            ExecutePayout(_cronobject);
        }
    }
}
