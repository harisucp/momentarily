using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.Controllers;
using Apeek.Common.Definitions;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Logger;
using Apeek.Core.Interfaces;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Messaging.Implementations;
using Apeek.Messaging.Interfaces;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Momentarily.Web.Models
{
    public class PaymentProcessClass : IJob
    {
        const string phPersonId = "{person-id}";
        const string phPersonName = "{person-name}";
        const string phLocation = "{location}";
        const string phServices = "{services}";

        private FromHeader _fromHeader;
        public FromHeader FromHeader { get { return _fromHeader; } set { _fromHeader = value; } }
        public PaymentProcessClass()
        {
            Settings = new SettingsDataService();
        }
        //public SendMessageService(FromHeader fromHeader) : this()
        //{
        //    _fromHeader = fromHeader;
        //}
        public bool SendVia(IMessageSendProvider messageSendProvider, IMessage message, ISendProperty sendProperty)
        {
            if (messageSendProvider.Auth(sendProperty))
            {
                return messageSendProvider.SendMessage(message);
            }
            return false;
        }
        private ISettingsDataService Settings { get; set; }



        protected RepositoryGoodRequest _repGoodRequest = new RepositoryGoodRequest();
        protected RepositoryPaypalInfoPaymentDetail _repositoryPaypalInfoPaymentDetail = new RepositoryPaypalInfoPaymentDetail();
        protected RepositoryUserGood _repositoryUserGood = new RepositoryUserGood();
        protected RepositoryFinalFeedback _repFinalFeedback = new RepositoryFinalFeedback();
        protected IRepositoryPaypalPayment _repositoryPaypalPayment = new RepositoryPaypalPayment();
        protected IRepositorySubscibes _repositorySubscibes = new RepositorySubscibes();
        protected IDbLogger _dbLogger = new DbLogger();
        protected IRepositoryLogEntry _repositoryLogEntry = new RepositoryLogEntry();
        //protected ISendMessageService _sendMessageService  = new SendMessageService();
        public async Task Execute(IJobExecutionContext context)
        {
            Uow.Wrap(u =>
            {
                string message = string.Empty;
                var AllPendingRequest = _repGoodRequest.Table.Where(x => x.StatusId == (int)UserRequestStatus.ReturnConfirmed
                     || x.StatusId == (int)UserRequestStatus.Late
                     || x.StatusId == (int)UserRequestStatus.Damaged
                     || x.StatusId == (int)UserRequestStatus.LateAndDamaged
                    ).ToList();

                if (AllPendingRequest != null && AllPendingRequest.Count > 0)
                {
                    foreach (var request in AllPendingRequest)
                    {

                        _repositoryLogEntry.SaveLogs("PayPalPaymentService", string.Format("Payment process started for request id " + request.Id), "MESSAGE");
                        //Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Payment process started for request id" + request.Id));
                     
                        try
                        {
                            var captureId = (from paymentDetail in _repositoryPaypalPayment.Table where paymentDetail.GoodRequestId == request.Id select paymentDetail.CaptureId).FirstOrDefault();
                            var borrowerId = request.UserId;
                            var bookingId = request.Id;
                            var sharerId = (from usergood in _repositoryUserGood.Table where usergood.GoodId == request.GoodId select usergood.UserId).FirstOrDefault();

                            var BorrowerPaypalDetail = (from inf in _repositoryPaypalInfoPaymentDetail.Table
                                                        where inf.UserId == borrowerId
                                                        select new PayoutDetailsModel
                                                        {
                                                            UserId = inf.UserId,
                                                            AccountNumber = inf.AccountNumber,
                                                            RoutingNumber = inf.RoutingNumber,
                                                            Locality = inf.Locality,
                                                            PostalCode = inf.PostalCode,
                                                            Region = inf.Region,
                                                            StreetAddress = inf.StreetAddress,
                                                            PaypalBusinessEmail = inf.PaypalBusinessEmail,
                                                            PaymentType = inf.PaymentType,
                                                            PhoneNumber = inf.PhoneNumber
                                                        }).FirstOrDefault();

                            var SharerPaypalDetail = (from inf in _repositoryPaypalInfoPaymentDetail.Table
                                                      where inf.UserId == sharerId
                                                      select new PayoutDetailsModel
                                                      {
                                                          UserId = inf.UserId,
                                                          AccountNumber = inf.AccountNumber,
                                                          RoutingNumber = inf.RoutingNumber,
                                                          Locality = inf.Locality,
                                                          PostalCode = inf.PostalCode,
                                                          Region = inf.Region,
                                                          StreetAddress = inf.StreetAddress,
                                                          PaypalBusinessEmail = inf.PaypalBusinessEmail,
                                                          PaymentType = inf.PaymentType,
                                                          PhoneNumber = inf.PhoneNumber
                                                      }).FirstOrDefault();

                            if (BorrowerPaypalDetail != null && SharerPaypalDetail != null)
                            {
                                if (request.StatusId == (int)UserRequestStatus.ReturnConfirmed)
                                {
                                    _repositoryLogEntry.SaveLogs("PayPalPaymentService", string.Format("Payment process started with no issue status for request id " + request.Id), "MESSAGE");
                                    //Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Payment process started with no issue status for request id" + request.Id));
                                    var payout = new Payout();
                                    payout.sender_batch_header = new PayoutSenderBatchHeader();
                                    payout.sender_batch_header.sender_batch_id = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                                    payout.sender_batch_header.email_subject = "Payment Processed. Request Id= " + request.Id;
                                    payout.items = new List<PayoutItem>();

                                    //var borroweritem = new PayoutItem();
                                    //var borroweramount = new Currency();
                                    //var _securityDepostit = request.SecurityDeposit;
                                    //_securityDepostit = Math.Round(_securityDepostit, 2);
                                    //borroweramount.value = Convert.ToString(_securityDepostit);
                                    //borroweramount.currency = "USD";
                                    //borroweritem.amount = borroweramount;

                                    //borroweritem.recipient_type = PayoutRecipientType.EMAIL;
                                    //borroweritem.receiver = BorrowerPaypalDetail.PaypalBusinessEmail;// borrower Paypal email id
                                    //borroweritem.note = "Payment To Borrower Account. Request Id " + request.Id;
                                    //borroweritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) +"Borrower_Good_Id" + request.GoodId;
                                    //payout.items.Add(borroweritem);

                                    //Refund Payment

                                    APIContext apiContext = PaypalConfiguration.GetAPIContext();
                                    PayPal.Api.Capture capture = PayPal.Api.Capture.Get(apiContext, captureId);
                                    Amount amount = new Amount();
                                    var _securityDepostit = request.SecurityDeposit;
                                    _securityDepostit = Math.Round(_securityDepostit, 2);
                                    amount.currency = "USD";
                                    amount.total = Convert.ToString(_securityDepostit);
                                    Refund refund = new Refund
                                    {
                                        amount = amount
                                    };

                                    var shareritem = new PayoutItem();
                                    var shareramount = new Currency();
                                    var _sSharerCost = request.SharerCost;
                                    _sSharerCost = Math.Round(_sSharerCost, 2);
                                    shareramount.value = Convert.ToString(_sSharerCost);
                                    shareramount.currency = "USD";
                                    shareritem.amount = shareramount;

                                    if (SharerPaypalDetail.PaymentType == (int)GlobalCode.Email)
                                    {
                                        shareritem.recipient_type = PayoutRecipientType.EMAIL;
                                        shareritem.receiver = SharerPaypalDetail.PaypalBusinessEmail;//sharer Paypal email id
                                    }
                                    else
                                    {
                                        shareritem.recipient_type = PayoutRecipientType.PHONE;
                                        shareritem.receiver = SharerPaypalDetail.PhoneNumber;//sharer Paypal email id
                                    }
                                    shareritem.note = "Payment To Sharer Account. Request Id " + request.Id;
                                    shareritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Sharer_Good Id" + request.GoodId;

                                    payout.items.Add(shareritem);

                                    //APIContext apiContext = PaypalConfiguration.GetAPIContext();
                                    var createdPayout = payout.Create(apiContext, false);
                                    var payoutDetail = Payout.Get(apiContext, createdPayout.batch_header.payout_batch_id);
                                    var refunded = capture.Refund(apiContext, refund);
                                    string state = refunded.state;
                                    request.StatusId = (int)UserRequestStatus.Closed;
                                    request.PendingAmount = 0.00;
                                    _repGoodRequest.Update(request);
                                    string userEmail = request.User.Email;
                                    bool checkExsistSubscriber = _repositorySubscibes.SubscriberAlreadyExsist(userEmail);
                                    if (checkExsistSubscriber == false)
                                    {
                                        string subscribertURL = "/Account/NewsLetterSubscribe?Email=" + userEmail;
                                        var sendMsgSubscriber = SendEmailNewsLetterTemplateForJobProcess(userEmail, subscribertURL);
                                        
                                    }
                                     SendEmailAfterTransationCompleteJobProcess(userEmail, request.User.FullName, bookingId);
                                    continue;
                                }

                                else if (request.StatusId == (int)UserRequestStatus.Late)
                                {

                                    _repositoryLogEntry.SaveLogs("PayPalPaymentService", string.Format("Payment process started with late status for request id " + request.Id), "MESSAGE");

                                    //Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Payment process started with late status for request id" + request.Id));
                                    var feedback = _repFinalFeedback.Table.Where(x => x.RequestId == request.Id).FirstOrDefault();
                                    if (feedback != null && feedback.ModDate.AddHours(48) <= DateTime.Now)
                                    {
                                        var ExpectedReturnDate = request.GoodBooking.EndDate.ToString("MM/dd/yyyy");
                                        var ExpectedReturnTime = request.GoodBooking.EndTime;
                                        DateTime ExpectedReturnDateTime = DateTime.ParseExact(ExpectedReturnDate + " " + ExpectedReturnTime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                                        var Actualreturndate = feedback.ReturnDate.ToString("MM/dd/yyyy");
                                        var Actualreturntime = feedback.ReturnTime;
                                        DateTime ActualReturnDateTime = DateTime.ParseExact(Actualreturndate + " " + Actualreturntime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                                        var Differences = ActualReturnDateTime - ExpectedReturnDateTime;

                                        var PerDayCost = request.DaysCost / request.Days;
                                        var PerHourCost = PerDayCost / 21;

                                        double DaysLateFee = 0.00;
                                        double HoursLateFee = 0.00;
                                        double TotalLateFee = 0.00;
                                        double PendingAmount = 0.00;
                                        DaysLateFee = Differences.Days * PerDayCost;

                                        if (Differences.Hours >= 21)
                                        {
                                            HoursLateFee = PerDayCost;
                                        }
                                        else
                                        {
                                            HoursLateFee = (Differences.Hours * 2) * PerHourCost;
                                            if (HoursLateFee > PerDayCost)
                                            {
                                                HoursLateFee = PerDayCost;
                                            }
                                        }
                                        TotalLateFee = DaysLateFee + HoursLateFee;
                                        if (TotalLateFee > request.SecurityDeposit)
                                        {
                                            PendingAmount = TotalLateFee - request.SecurityDeposit;
                                            TotalLateFee = request.SecurityDeposit;
                                        }

                                        var payout = new Payout();
                                        payout.sender_batch_header = new PayoutSenderBatchHeader();
                                        payout.sender_batch_header.sender_batch_id = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                                        payout.sender_batch_header.email_subject = "Payment Processed with late fee. Request Id= " + request.Id;
                                        payout.items = new List<PayoutItem>();

                                        //var borroweritem = new PayoutItem();
                                        //var borroweramount = new Currency();
                                        //if (TotalLateFee != request.SecurityDeposit)
                                        //{
                                        //    var _borrowerAmount = request.SecurityDeposit - TotalLateFee;
                                        //    _borrowerAmount = Math.Round(_borrowerAmount, 2);
                                        //    borroweramount.value = Convert.ToString(_borrowerAmount);
                                        //    borroweramount.currency = "USD";
                                        //    borroweritem.amount = borroweramount;

                                        //    borroweritem.recipient_type = PayoutRecipientType.EMAIL;
                                        //    borroweritem.receiver = BorrowerPaypalDetail.PaypalBusinessEmail;// borrower Paypal email id
                                        //    borroweritem.note = "Payment To Borrower Account with late fee. Request Id " + request.Id;
                                        //    borroweritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Borrower_Good_Id" + request.GoodId;
                                        //    payout.items.Add(borroweritem);
                                        //}
                                        APIContext apiContext = PaypalConfiguration.GetAPIContext();
                                        PayPal.Api.Capture capture = PayPal.Api.Capture.Get(apiContext, captureId);
                                        Refund refund = new Refund();
                                        if (TotalLateFee != request.SecurityDeposit)
                                        {
                                            Amount amount = new Amount();
                                            var _borrowerAmount = request.SecurityDeposit - TotalLateFee;
                                            _borrowerAmount = Math.Round(_borrowerAmount, 2);
                                            amount.currency = "USD";
                                            amount.total = Convert.ToString(_borrowerAmount);
                                            refund.amount = amount;

                                        }
                                        var shareritem = new PayoutItem();
                                        var shareramount = new Currency();
                                        var _sharerAmount = request.SharerCost + TotalLateFee;
                                        _sharerAmount = Math.Round(_sharerAmount, 2);
                                        shareramount.value = Convert.ToString(_sharerAmount);
                                        shareramount.currency = "USD";
                                        shareritem.amount = shareramount;

                                        if (SharerPaypalDetail.PaymentType == (int)GlobalCode.Email)
                                        {
                                            shareritem.recipient_type = PayoutRecipientType.EMAIL;
                                            shareritem.receiver = SharerPaypalDetail.PaypalBusinessEmail;//sharer Paypal email id
                                        }
                                        else
                                        {
                                            shareritem.recipient_type = PayoutRecipientType.PHONE;
                                            shareritem.receiver = SharerPaypalDetail.PhoneNumber;//sharer Paypal email id
                                        }


                                        shareritem.note = "Payment To sharer Account with late fee. Request Id " + request.Id;

                                        shareritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Sharer_Good_Id" + request.GoodId;


                                        payout.items.Add(shareritem);
                                        //APIContext apiContext = PaypalConfiguration.GetAPIContext();
                                        var createdPayout = payout.Create(apiContext, false);
                                        var payoutDetail = Payout.Get(apiContext, createdPayout.batch_header.payout_batch_id);
                                        if (TotalLateFee != request.SecurityDeposit)
                                        {
                                            var refunded = capture.Refund(apiContext, refund);
                                            string state = refunded.state;
                                        }
                                        request.StatusId = (int)UserRequestStatus.ClosedWithLate;
                                        request.PendingAmount = Math.Round(PendingAmount, 2);
                                        _repGoodRequest.Update(request);
                                        string userEmail = request.User.Email;
                                        bool checkExsistSubscriber = _repositorySubscibes.SubscriberAlreadyExsist(userEmail);
                                        if (checkExsistSubscriber == false)
                                        {
                                            string subscribertURL = "/Account/NewsLetterSubscribe?Email=" + userEmail;
                                            var sendMsgSubscriber = SendEmailNewsLetterTemplateForJobProcess(userEmail, subscribertURL);
                                        }
                                        SendEmailAfterTransationCompleteJobProcess(userEmail, request.User.FullName, bookingId);
                                        continue;
                                    }

                                }

                                else if (request.StatusId == (int)UserRequestStatus.Damaged)
                                {
                                    _repositoryLogEntry.SaveLogs("PayPalPaymentService", string.Format("Payment process started with damaged status for request id " + request.Id), "MESSAGE");
                                    //Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Payment process started with damaged status for request id" + request.Id));

                                    var feedback = _repFinalFeedback.Table.Where(x => x.RequestId == request.Id).FirstOrDefault();
                                    if (feedback != null && feedback.ModDate.AddHours(48) <= DateTime.Now)
                                    {
                                        double TotalClaim = feedback.Claim;
                                        double PendingAmount = 0.00;

                                        if (TotalClaim > request.SecurityDeposit)
                                        {
                                            PendingAmount = TotalClaim - request.SecurityDeposit;
                                            TotalClaim = request.SecurityDeposit;
                                        }

                                        var payout = new Payout();
                                        payout.sender_batch_header = new PayoutSenderBatchHeader();
                                        payout.sender_batch_header.sender_batch_id = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                                        payout.sender_batch_header.email_subject = "Payment Processed with damaged fee. Request Id= " + request.Id;
                                        payout.items = new List<PayoutItem>();

                                        //var borroweritem = new PayoutItem();
                                        //var borroweramount = new Currency();
                                        //if (TotalClaim != request.SecurityDeposit)
                                        //{
                                        //    var _borrowerAmount = request.SecurityDeposit - TotalClaim;
                                        //    _borrowerAmount = Math.Round(_borrowerAmount, 2);
                                        //    borroweramount.value = Convert.ToString(_borrowerAmount);
                                        //    borroweramount.currency = "USD";
                                        //    borroweritem.amount = borroweramount;

                                        //    borroweritem.recipient_type = PayoutRecipientType.EMAIL;
                                        //    borroweritem.receiver = BorrowerPaypalDetail.PaypalBusinessEmail;// borrower Paypal email id
                                        //    borroweritem.note = "Payment To Borrower Account with Damaged fee. Request Id " + request.Id;
                                        //    borroweritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Borrower_Good_Id" + request.GoodId;
                                        //    payout.items.Add(borroweritem);
                                        //}
                                        APIContext apiContext = PaypalConfiguration.GetAPIContext();
                                        PayPal.Api.Capture capture = PayPal.Api.Capture.Get(apiContext, captureId);
                                        Refund refund = new Refund();
                                        if (TotalClaim != request.SecurityDeposit)
                                        {
                                            Amount amount = new Amount();
                                            var _borrowerAmount = request.SecurityDeposit - TotalClaim;
                                            _borrowerAmount = Math.Round(_borrowerAmount, 2);
                                            amount.currency = "USD";
                                            amount.total = Convert.ToString(_borrowerAmount);
                                            refund.amount = amount;

                                        }

                                        var shareritem = new PayoutItem();
                                        var shareramount = new Currency();
                                        var _sharerAmount = request.SharerCost + TotalClaim;
                                        _sharerAmount = Math.Round(_sharerAmount, 2);
                                        shareramount.value = Convert.ToString(_sharerAmount);
                                        shareramount.currency = "USD";
                                        shareritem.amount = shareramount;

                                        if (SharerPaypalDetail.PaymentType == (int)GlobalCode.Email)
                                        {
                                            shareritem.recipient_type = PayoutRecipientType.EMAIL;
                                            shareritem.receiver = SharerPaypalDetail.PaypalBusinessEmail;//sharer Paypal email id
                                        }
                                        else
                                        {
                                            shareritem.recipient_type = PayoutRecipientType.PHONE;
                                            shareritem.receiver = SharerPaypalDetail.PhoneNumber;//sharer Paypal email id
                                        }
                                        shareritem.note = "Payment To sharer Account with damaged fee. Request Id " + request.Id;
                                        shareritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Sharer_Good_Id" + request.GoodId;
                                        payout.items.Add(shareritem);

                                        //APIContext apiContext = PaypalConfiguration.GetAPIContext();
                                        var createdPayout = payout.Create(apiContext, false);
                                        var payoutDetail = Payout.Get(apiContext, createdPayout.batch_header.payout_batch_id);
                                        if (TotalClaim != request.SecurityDeposit)
                                        {
                                            var refunded = capture.Refund(apiContext, refund);
                                            string state = refunded.state;
                                        }
                                        request.StatusId = (int)UserRequestStatus.ClosedWithDamaged;
                                        request.PendingAmount = Math.Round(PendingAmount, 2);
                                        _repGoodRequest.Update(request);
                                        string userEmail = request.User.Email;
                                        bool checkExsistSubscriber = _repositorySubscibes.SubscriberAlreadyExsist(userEmail);
                                        if (checkExsistSubscriber == false)
                                        {
                                            string subscribertURL = "/Account/NewsLetterSubscribe?Email=" + userEmail;
                                            var sendMsgSubscriber = SendEmailNewsLetterTemplateForJobProcess(userEmail, subscribertURL);
                                        }
                                       SendEmailAfterTransationCompleteJobProcess(userEmail, request.User.FullName, bookingId);
                                        continue;
                                    }
                                }

                                else if (request.StatusId == (int)UserRequestStatus.LateAndDamaged)
                                {
                                    _repositoryLogEntry.SaveLogs("PayPalPaymentService", string.Format("Payment process started with late and damaged status for request id " + request.Id), "MESSAGE");
                                   // Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Payment process started with late and damaged status for request id" + request.Id));

                                    var feedback = _repFinalFeedback.Table.Where(x => x.RequestId == request.Id).FirstOrDefault();
                                    if (feedback != null && feedback.ModDate.AddHours(48) <= DateTime.Now)
                                    {

                                        var ExpectedReturnDate = request.GoodBooking.EndDate.ToString("MM/dd/yyyy");
                                        var ExpectedReturnTime = request.GoodBooking.EndTime;
                                        DateTime ExpectedReturnDateTime = DateTime.ParseExact(ExpectedReturnDate + " " + ExpectedReturnTime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                                        var Actualreturndate = feedback.ReturnDate.ToString("MM/dd/yyyy");
                                        var Actualreturntime = feedback.ReturnTime;
                                        DateTime ActualReturnDateTime = DateTime.ParseExact(Actualreturndate + " " + Actualreturntime, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

                                        var Differences = ActualReturnDateTime - ExpectedReturnDateTime;

                                        var PerDayCost = request.DaysCost / request.Days;
                                        var PerHourCost = PerDayCost / 21;
                                        double DaysLateFee = 0.00;
                                        double HoursLateFee = 0.00;
                                        double TotalLateFee = 0.00;
                                        double PendingAmount = 0.00;
                                        double TotalClaim = feedback.Claim;
                                        DaysLateFee = Differences.Days * PerDayCost;

                                        if (Differences.Hours >= 21)
                                        {
                                            HoursLateFee = PerDayCost;
                                        }
                                        else
                                        {
                                            HoursLateFee = (Differences.Hours * 2) * PerHourCost;
                                            if (HoursLateFee > PerDayCost)
                                            {
                                                HoursLateFee = PerDayCost;
                                            }
                                        }
                                        TotalLateFee = DaysLateFee + HoursLateFee;
                                        double TotalRecoveryAmount = TotalLateFee + TotalClaim;

                                        if (TotalRecoveryAmount > request.SecurityDeposit)
                                        {
                                            PendingAmount = TotalRecoveryAmount - request.SecurityDeposit;
                                            TotalRecoveryAmount = request.SecurityDeposit;
                                        }

                                        var payout = new Payout();
                                        payout.sender_batch_header = new PayoutSenderBatchHeader();
                                        payout.sender_batch_header.sender_batch_id = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8);
                                        payout.sender_batch_header.email_subject = "Payment Processed with damaged and late fee. Request Id= " + request.Id;
                                        payout.items = new List<PayoutItem>();

                                        //var borroweritem = new PayoutItem();
                                        //var borroweramount = new Currency();
                                        //if (TotalRecoveryAmount != request.SecurityDeposit)
                                        //{
                                        //    var _borrowerAmount = request.SecurityDeposit - TotalRecoveryAmount;
                                        //    _borrowerAmount = Math.Round(_borrowerAmount, 2);
                                        //    borroweramount.value = Convert.ToString(_borrowerAmount);
                                        //    borroweramount.currency = "USD";
                                        //    borroweritem.amount = borroweramount;

                                        //    borroweritem.recipient_type = PayoutRecipientType.EMAIL;
                                        //    borroweritem.receiver = BorrowerPaypalDetail.PaypalBusinessEmail;// borrower Paypal email id
                                        //    borroweritem.note = "Payment To Borrower Account with Damaged and late fee. Request Id " + request.Id;
                                        //    borroweritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Borrower_Good_Id" + request.GoodId;
                                        //    payout.items.Add(borroweritem);
                                        //}
                                        APIContext apiContext = PaypalConfiguration.GetAPIContext();
                                        PayPal.Api.Capture capture = PayPal.Api.Capture.Get(apiContext, captureId);
                                        Refund refund = new Refund();
                                        if (TotalRecoveryAmount != request.SecurityDeposit)
                                        {
                                            Amount amount = new Amount();
                                            var _borrowerAmount = request.SecurityDeposit - TotalRecoveryAmount;
                                            _borrowerAmount = Math.Round(_borrowerAmount, 2);
                                            amount.currency = "USD";
                                            amount.total = Convert.ToString(_borrowerAmount);
                                            refund.amount = amount;

                                        }

                                        var shareritem = new PayoutItem();
                                        var shareramount = new Currency();
                                        var _sharerAmount = request.SharerCost + TotalRecoveryAmount;
                                        _sharerAmount = Math.Round(_sharerAmount, 2);
                                        shareramount.value = Convert.ToString(_sharerAmount);
                                        shareramount.currency = "USD";
                                        shareritem.amount = shareramount;

                                        if (SharerPaypalDetail.PaymentType == (int)GlobalCode.Email)
                                        {
                                            shareritem.recipient_type = PayoutRecipientType.EMAIL;
                                            shareritem.receiver = SharerPaypalDetail.PaypalBusinessEmail;//sharer Paypal email id
                                        }
                                        else
                                        {
                                            shareritem.recipient_type = PayoutRecipientType.PHONE;
                                            shareritem.receiver = SharerPaypalDetail.PhoneNumber;//sharer Paypal email id
                                        }
                                        shareritem.note = "Payment To sharer Account with damaged and late fee. Request Id " + request.Id;
                                        shareritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Sharer_Good_Id" + request.GoodId;
                                        payout.items.Add(shareritem);

                                        //APIContext apiContext = PaypalConfiguration.GetAPIContext();
                                        var createdPayout = payout.Create(apiContext, false);
                                        var payoutDetail = Payout.Get(apiContext, createdPayout.batch_header.payout_batch_id);
                                        if (TotalRecoveryAmount != request.SecurityDeposit)
                                        {
                                            var refunded = capture.Refund(apiContext, refund);
                                            string state = refunded.state;
                                        }
                                        request.StatusId = (int)UserRequestStatus.ClosedWithLateAndDamaged;
                                        request.PendingAmount = Math.Round(PendingAmount, 2);
                                        _repGoodRequest.Update(request);
                                        string userEmail = request.User.Email;
                                        bool checkExsistSubscriber = _repositorySubscibes.SubscriberAlreadyExsist(userEmail);
                                        if (checkExsistSubscriber == false)
                                        {
                                            string subscribertURL = "/Account/NewsLetterSubscribe?Email=" + userEmail;
                                            var sendMsgSubscriber = SendEmailNewsLetterTemplateForJobProcess(userEmail, subscribertURL);
                                        }
                                         SendEmailAfterTransationCompleteJobProcess(userEmail, request.User.FullName, bookingId);
                                        continue;
                                    }
                                }
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            _repositoryLogEntry.SaveLogs("PayPalPaymentService", string.Format("Create pending request fail for request id " + request.Id), "ERROR");
                           // Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Create pending request fail for request id" + request.Id + ". Ex: {0}.", ex));
                            continue;
                        }
                    }
                }
            },
                null, LogSource.GoodRequestService);
        }


        public bool SendEmailNewsLetterTemplateForJobProcess(string userEmail, string url)
        {
            try
            {
               
                string domainNameJob = "https://www.momentarily.com";
                string currentYear = Convert.ToString(DateTime.Now.Year);
                string linkUrl = domainNameJob + url;
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = "Newsletter"
                    };
                }
                message.To = userEmail;
                message.Body = getmailTemplate(Language.en, "NewsLetter");
                message.Body = message.Body.Replace("my_hostings", domainNameJob);
                message.Body = message.Body.Replace("buttonLink", linkUrl);
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.Body = message.Body.Replace("textEmail", "subscribeEmail");

                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendPropertyForMarketing());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", userEmail), ex);
            }
            return false;
        }

        public bool SendEmailAfterTransationCompleteJobProcess(string email, string borrowerName,int bookingId)
        {
            const string phBorrowerName = "{borrower}";
            string domainName = "https://www.momentarily.com";
            string currentYear = Convert.ToString(DateTime.Now.Year);
            try
            {
                var t = Ioc.Get<ITranslateController>();
                var message = new Message();
                if (_fromHeader != null)
                    message.FromHeader = _fromHeader;
                else
                {
                    message.FromHeader = new FromHeader()
                    {
                        From = Settings.GetSmtpFrom(),
                        FromName = Settings.GetSmtpFromName(),
                        Subject = "Transaction Complete"
                    };
                }
                message.To = email;
                message.Body = MessageTemplate.GetMailTemplate(Language.en, "transaction_complete");

                message.Body = message.Body.Replace("my_hostings", domainName);
                message.Body = message.Body.Replace(phBorrowerName, borrowerName);
                message.Body = message.Body.Replace("{BookingId}", Convert.ToString(bookingId));
                message.Body = message.Body.Replace("CurrentYear", currentYear);
                message.IsBodyHtml = true;
                return SendVia(Ioc.Get<IMailSendProvider>(), message, new SettingsDataService().GetEmailSendProperty());
            }
            catch (Exception ex)
            {
                Ioc.Get<DbLogger>().LogException(LogSource.SendMail, string.Format("Error when sending verification message to email: {0}", email), ex);
            }
            return false;
        }


        private string getmailTemplate(Language lang, string templateName)
        {
            string path = string.Format(@"{0}\mails\{1}\{2}.cshtml", AppSettings.GetInstance().AppdataDirectory, lang, templateName);
            return File.ReadAllText(path);
        }
    }
}