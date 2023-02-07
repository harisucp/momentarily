using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.NH.Repository.Repositories.Impl;
using Momentarily.Core.Services.Impl;
using Momentarily.UI.Service.Services;
using Momentarily.UI.Service.Services.Impl;
using Momentarily.ViewModels.Models.Braintree;
using Momentarily.Web.Areas.Frontend.Controller;
using PayPal.Api;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace Momentarily.Web.Models
{
    public class CapturePendingPayments : IJob
    {
        protected RepositoryGoodRequest _repGoodRequest = new RepositoryGoodRequest();
        protected IRepositoryPaypalPayment _repositoryPaypalPayment = new RepositoryPaypalPayment();
        protected IRepositoryLogEntry _repositoryLogEntry = new RepositoryLogEntry();
        public async Task Execute(IJobExecutionContext context)
        {
            Uow.Wrap(u =>
            {

                APIContext apiContext = PaypalConfiguration.GetAPIContext();
                var allPayments = _repositoryPaypalPayment.Table.ToList();
                foreach (var payment in allPayments)
                {
                    try
                    {
                       
                        var Request = _repGoodRequest.GetUserRequest(payment.GoodRequestId);
                        if (Request != null && (Request.StatusId == (int)UserRequestStatus.Paid || Request.StatusId == (int)UserRequestStatus.Released
                                || Request.StatusId == (int)UserRequestStatus.Received || Request.StatusId == (int)UserRequestStatus.Returned
                                || Request.StatusId == (int)UserRequestStatus.ReturnConfirmed || Request.StatusId == (int)UserRequestStatus.Late
                                || Request.StatusId == (int)UserRequestStatus.Damaged || Request.StatusId == (int)UserRequestStatus.LateAndDamaged))
                        {
                            DateTime capture_date = payment.ModDate.AddDays(29);

                            if (capture_date.ToString("MM/dd/yyyy") == DateTime.Now.ToString("MM/dd/yyyy"))
                            {
                                _repositoryLogEntry.SaveLogs("PayPalPaymentService", string.Format("Capture pending payment process for payment id " + payment.Id), "MESSAGE");
                                PayPal.Api.Authorization authorize = PayPal.Api.Authorization.Get(apiContext, payment.AuthorizationId);
                                var capture = new Capture()
                                {
                                    amount = new Amount()
                                    {
                                        currency = "USD",
                                        total = Convert.ToString(Request.CustomerCost + Request.SecurityDeposit)
                                    },
                                    is_final_capture = false
                                };

                                var reCapture = authorize.Capture(apiContext, capture);

                                payment.AuthorizationCreatedDate = Convert.ToDateTime(authorize.create_time);
                                payment.AuthorizationId = authorize.id;
                                payment.AuthorizationCount = payment.AuthorizationCount + 1;
                                payment.AuthorizationStatus = authorize.state;
                                payment.AuthorizationUpdateDate = Convert.ToDateTime(authorize.update_time);
                                payment.AuthorizationValidUntill = Convert.ToDateTime(authorize.valid_until);
                                payment.CaptureCreatedDate = Convert.ToDateTime(reCapture.create_time);
                                payment.CaptureId = reCapture.id;
                                payment.CaptureIsFinal = false;
                                payment.CaptureStatus = reCapture.state;
                                payment.CaptureUpdateDate = Convert.ToDateTime(reCapture.update_time);
                                payment.ModDate = DateTime.Now;
                                _repositoryPaypalPayment.SaveOrUpdate(payment);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        _repositoryLogEntry.SaveLogs("PayPalPaymentService", string.Format("Capture pending payment process fail for payment id " + payment.Id), "ERROR");
                        //Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Capture pending payment process fail for payment id" + payment.Id));
                        continue;
                    }
                }
            },
                null, LogSource.PayPalPaymentService);
        }
    }
}