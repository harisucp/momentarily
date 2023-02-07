using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Models.Impl;
using Apeek.ViewModels.Settings;
using Momentarily.NH.Repository.Repositories;
using Momentarily.ViewModels.Models;
using PinPayments;
using PinPayments.Actions;
using PinPayments.Infrastructure;
using PinPayments.Models;
namespace Momentarily.UI.Service.Services.Impl
{
    public class PinPaymentService
    {
        private readonly ISettingsDataService _settingsService;
        private readonly PinService _pinService;
        private readonly PinPaymentPublicKeys _pinPaymentPublicKey;
        private readonly IMomentarilyGoodRequestService _goodRequestService;
        private readonly IAccountDataService _accountDataService;
        private readonly IMomentarilyItemDataService _goodDataService;
        private readonly IRepositoryPaymentTransaction _repPaymentTransaction;
        private readonly IMomentarilyUserMessageService _userMessageService;
        private readonly ISendMessageService _sendMessageService;
        public PinPaymentService()
        {
            _settingsService = Ioc.Get<ISettingsDataService>();
            _accountDataService = Ioc.Get<IAccountDataService>();
            _goodRequestService = Ioc.Get<IMomentarilyGoodRequestService>();
            _goodDataService = Ioc.Get<IMomentarilyItemDataService>();
            _repPaymentTransaction = Ioc.Get<IRepositoryPaymentTransaction>();
            _userMessageService = Ioc.Get<IMomentarilyUserMessageService>();
            _sendMessageService = Ioc.Get<ISendMessageService>();
            PinPaymentSetting setting = _settingsService.GetPinPaymentSetting();
            //TODO: load keys from DB
            Urls urls = new Urls(setting.TestMode ? "https://test-api.pin.net.au" : "https://api.pin.net.au");
            _pinService = new PinService(setting.TestMode ? setting.TestSecretKey : setting.LiveSecretKey);
            _pinPaymentPublicKey = new PinPaymentPublicKeys { Key = setting.TestMode ? setting.TestPublishKey : setting.LivePublishKey, Mode = setting.TestMode ? "test" : "live" };
            _pinService.Urls = urls;
        }
        public PinPaymentPublicKeys PinPaymentPublicKey
        {
            get { return _pinPaymentPublicKey; }
        }
        public int AuthorizeDayExpire
        {
            get { return 5; }
        }
        public Result<PaymentResult> ChargeRequestAmount(int goodRequestId, int userId, string cardToken)
        {
            var result = new Result<PaymentResult>(CreateResult.Error, new PaymentResult() { IsError = true });
            var commision = _settingsService.GetBorrowerPaymentTransactionCommision();
            try
            {
                //TODO: USE Uow, add to AddBookingTransaction (Uow)
                var goodRequest = _goodRequestService.GetUserRequest(userId, goodRequestId);
                var user = _accountDataService.GetUser(userId);
                if (goodRequest.CreateResult == CreateResult.Success && user != null)
                {
                    var good = _goodDataService.GetItem(goodRequest.Obj.GoodId);
                    if (good.CreateResult == CreateResult.Success && goodRequest.Obj.StatusId == (int)UserRequestStatus.Approved)
                    {
                        var postCharge = new PostCharge()
                        {
                            CardToken = cardToken,
                            Description = "Booking for listing " + goodRequest.Obj.GoodName,
                            Amount = (long)((goodRequest.Obj.DaysCost + goodRequest.Obj.DaysCost * commision) * 100),
                            Email = user.Email
                        };
                        try
                        {
                            var response = _pinService.Charge(postCharge);
                            if (response.Charge.Success)
                            {
                                AddBookingTransaction(response, goodRequest.Obj.DaysCost, goodRequest.Obj.DaysCost * commision,
                                    goodRequest.Obj.Id, userId, PaymentTransactionType.SaleType);
                                // charge deposit
                                var r = Authorize(good.Obj.Deposit,
                                    "Security deposit for listing " + goodRequest.Obj.GoodName, user.Email, cardToken,
                                    goodRequest.Obj.Id, userId);
                                if (r)
                                {
                                    result.CreateResult = CreateResult.Success;
                                    result.Obj.IsError = false;
                                }
                            }
                        }
                        catch (PinException ex)
                        {
                            result.CreateResult = CreateResult.PaymentError;
                            result.Obj.Error = ex.PinError.Description;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Charge for request: {0} from card {1} fail. Ex: {2}.", goodRequestId, cardToken, ex));
            }
            return result;
        }
        private bool Authorize(double cost, string description, string email, string cardToken, int requestId, int userId, IUnitOfWork unitOfWork = null)
        {
            var postDepositCharge = new PostCharge()
            {
                Capture = false,
                CardToken = cardToken,
                Description = description,
                Amount = (long)cost * 100,
                Email = email
            };
            var responseSecurityDeposit = _pinService.Charge(postDepositCharge);
            if (responseSecurityDeposit.Charge.Success)
            {
                return AddBookingTransaction(responseSecurityDeposit, cost, 0, requestId, userId, PaymentTransactionType.AuthorizeType, unitOfWork);
            }
            return false;
        }
        public Result<Customer> AddCustomerToPinPayment(string email, string cardToken)
        {
            var result = new Result<Customer>(CreateResult.Error, null);
            try
            {
                var customer = new Customer()
                {
                    Email = email
                };
                if (!String.IsNullOrEmpty(cardToken))
                {
                    customer.CardToken = cardToken;
                }
                var customerResult = _pinService.CustomerAdd(customer);
                if (customerResult.Response != null)
                {
                    result.CreateResult = CreateResult.Success;
                    result.Obj = customerResult.Response;
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Create customers with email: {0} fail. Ex: {1}.", email, ex));
            }
            return result;
        }
        public Result<bool> AddCardToCustomer(string customerToken, string cardToken)
        {
            var result = new Result<bool>(CreateResult.Error, false);
            try
            {
                _pinService.AddCard(customerToken, cardToken);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Add card: {0} to  customer: {1} fail. Ex: {2}.", customerToken, cardToken, ex));
            }
            return result;
        }
        private bool AddBookingTransaction(ChargeResponse pinCharge, double amount, double commision, int goodRequestId, int userId, string type, IUnitOfWork unitOfWork = null)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var paymentTransaction = new PaymentTransaction
                    {
                        TransactionId = pinCharge.Charge.Token,
                        PayerId = pinCharge.Charge.Card.Token,
                        GoodRequestId = goodRequestId,
                        Type = type,
                        Cost = amount,
                        Commision = commision,
                        //CapureId = pinCharge.Charge.,
                        StatusId = (int)PaymentTransactionStatus.Pending
                    };
                    _repPaymentTransaction.SaveOrUpdateAudit(paymentTransaction, userId);
                    result = true;
                }, unitOfWork, LogSource.PinPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Save transaction for request: {0} TransactionId {1} fail. Ex: {2}.", goodRequestId, pinCharge.Charge.Token, ex));
            }
            return result;
        }
        private bool AddTransferTransaction(ApeekPayout payout, string transferToken)
        {
            var result = false;
            try
            {
                Uow.Wrap(u =>
                {
                    var paymentTransaction = new PaymentTransaction
                    {
                        TransactionId = transferToken,
                        PayerId = "",
                        GoodRequestId = payout.GoodRequestId,
                        Type = PaymentTransactionType.Payout,
                        Cost = payout.Amount,
                        Commision = 0,
                        StatusId = (int)PaymentTransactionStatus.Pending
                    };
                    _repPaymentTransaction.SaveOrUpdateAudit(paymentTransaction, 0);
                    _goodRequestService.SharerTakeUserRequest(payout.GoodRequestUserId, payout.GoodRequestId, u);
                }, null, LogSource.PinPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Save transfer transaction for request: {0} TransactionId {1} fail. Ex: {2}.", payout.GoodRequestId, transferToken, ex));
            }
            return result;
        }
        public void SendNotificationEmails(int goodRequestId, int seekerId, QuickUrl quickUrl)
        {
            try
            {
                var goodRequest = _goodRequestService.GetUserRequest(seekerId, goodRequestId);
                var good = _goodDataService.GetItem(goodRequest.Obj.GoodId);
                var sharerId = good.Obj.User.Id;
                _userMessageService.SendPayGoodRequestMessage(seekerId, sharerId, goodRequestId, quickUrl);
                _sendMessageService.SendEmailPayRequestMessage(good.Obj.User.Email, good.Obj.Name,
                    quickUrl.GoodRequestAbsoluteUrl(goodRequestId));
                var seekerUserInfo = _accountDataService.GetUser(seekerId);
                var sharerUserInfo = _accountDataService.GetUser(good.Obj.User.Id);
                _sendMessageService.SendConfirmEmailToSharer(sharerUserInfo.Email,
                    good.Obj.Name,
                    goodRequest.Obj.StartDate.ToString("dd/MM/yyyy"),
                    goodRequest.Obj.EndDate.ToString("dd/MM/yyyy"),
                    goodRequest.Obj.DaysCost.ToString("F2", CultureInfo.InvariantCulture),
                    good.Obj.PickUpLoaction,
                    new UserContactInfo
                    {
                        Email = seekerUserInfo.Email,
                        Name = seekerUserInfo.FirstName,
                        Phone = _accountDataService.GetUserPhone(seekerId)
                    }, quickUrl.GoodRequestAbsoluteUrl(goodRequestId));
                _sendMessageService.SendConfirmEmailToSeeker(seekerUserInfo.Email,
                    good.Obj.Name,
                    goodRequest.Obj.StartDate.ToString("dd/MM/yyyy"),
                    goodRequest.Obj.EndDate.ToString("dd/MM/yyyy"),
                    goodRequest.Obj.DaysCost.ToString("F2", CultureInfo.InvariantCulture),
                    (goodRequest.Obj.CustomerServiceFee * goodRequest.Obj.DaysCost).ToString("F2", CultureInfo.InvariantCulture),
                    good.Obj.Deposit.ToString("F2", CultureInfo.InvariantCulture),
                    (goodRequest.Obj.DaysCost + goodRequest.Obj.CustomerServiceFee * goodRequest.Obj.DaysCost).ToString("F2",
                        CultureInfo.InvariantCulture),
                    good.Obj.PickUpLoaction,
                    new UserContactInfo
                    {
                        Email = sharerUserInfo.Email,
                        Name = sharerUserInfo.FirstName,
                        Phone = _accountDataService.GetUserPhone(good.Obj.User.Id)
                    },
                    quickUrl.UserRequestAbsoluteUrl(goodRequestId)
                    );
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Send emails for request: {0} fail. Ex: {1}.", goodRequestId, ex));
            }
        }
        public Result<Recipient> AddOrUpdateRecipient(string email, string name, UserBankInfoViewModel bankModel, string recipientToken = null)
        {
            if (String.IsNullOrEmpty(recipientToken))
            {
                return AddRecipient(email, name, bankModel);
            }
            else
            {
                return UpdateRecipient(email, name, recipientToken, bankModel);
            }
        }
        private Result<Recipient> AddRecipient(string email, string name, UserBankInfoViewModel bankModel)
        {
            var result = new Result<Recipient>(CreateResult.Error, null);
            try
            {
                var recipientPost = new RecipientPost()
                {
                    Name = name,
                    Email = email,
                    Bank = new Bank() { BSB = bankModel.BSB1 + bankModel.BSB2, Name = bankModel.Name, Number = bankModel.Number }
                };
                var resultRecipient = _pinService.PostRecipient(recipientPost);
                result.Obj = resultRecipient.Response;
                result.CreateResult = CreateResult.Success;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Create recipient with email: {0} fail. Ex: {1}.", email, ex));
            }
            return result;
        }
        private Result<Recipient> UpdateRecipient(string email, string name, string recipientToken, UserBankInfoViewModel bankModel)
        {
            var result = new Result<Recipient>(CreateResult.Error, null);
            try
            {
                var recipientPost = new RecipientPost()
                {
                    Name = name,
                    Email = email,
                    Bank = new Bank() { BSB = bankModel.BSB1 + bankModel.BSB2, Name = bankModel.Name, Number = bankModel.Number }
                };
                var resultRecipient = _pinService.UpdateRecipient(recipientPost, recipientToken);
                result.Obj = resultRecipient.Response;
                result.CreateResult = CreateResult.Success;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Update recipient with email: {0} fail. Ex: {1}.", email, ex));
            }
            return result;
        }
        public Result<Recipient> GetRecipientInfo(string token)
        {
            var result = new Result<Recipient>(CreateResult.Error, null);
            try
            {
                var recipient = _pinService.Recipient(token);
                result.Obj = recipient.Response;
                result.CreateResult = CreateResult.Success;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Get recipient info for token: {0} fail. Ex: {1}.", token, ex));
            }
            return result;
        }
        public Result<bool> Payout(ApeekPayout payout)
        {
            var result = new Result<bool>(CreateResult.Error, false);
            try
            {
                if (!String.IsNullOrEmpty(payout.RecipientToken))
                {
                    var transferPost = new TransferPost()
                    {
                        Amount = (int)(payout.Amount * 100),
                        Description = String.Format(@"Payout for booking {0}", payout.GoodName),
                        RecipientToken = payout.RecipientToken
                    };
                    var resultTransfer = _pinService.PostTransfer(transferPost);
                    if (resultTransfer.Response != null)
                    {
                        AddTransferTransaction(payout, resultTransfer.Response.Token);
                        Ioc.Get<IDbLogger>().LogMessage(LogSource.PinPaymentService, string.Format("Payout {0} cost to user id: {1} for good request: {2}", payout.Amount, payout.UserId, payout.GoodRequestId));
                        //_goodRequestService.SharerTakeUserRequest(payout.UserId, payout.GoodRequestId);
                    }
                }
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Payout {0} cost to user id: {1} for good request: {2} fail. Ex: {3}.", payout.Amount, payout.UserId, payout.GoodRequestId, ex));
            }
            return result;
        }
        public Result<List<Charge>> GetAuthorizeExpireChargesByDate(DateTime date)
        {
            var result = new Result<List<Charge>>(CreateResult.Error, new List<Charge>());
            try
            {
                //var startDate = date.Date;
                //for test 
                var startDate = date.Date.AddDays(-5);
                var endDate = date.Date.AddDays(1);
                var chargesSearch = new ChargeSearch()
                {
                    StartDate = startDate,
                    EndDate = endDate
                };
                Charges response = null;
                int? page = 1;
                do
                {
                    response = _pinService.ChargesSearch(chargesSearch, page);
                    var listOfCharge = response.Response.ToList();
                    //TODO: test. Change AuthorisationExpired to true
                    listOfCharge.Where(c => c.Captured == false)
                        .ToList()
                        .ForEach(c => { c.AuthorisationExpired = true; });
                    //
                    result.Obj.AddRange(listOfCharge.Where(r => r.AuthorisationExpired == true && r.Captured == false));
                    if (response.Pagination.Next.HasValue)
                    {
                        page = response.Pagination.Next.Value;
                    }
                } while (response.Pagination.Next.HasValue);
                result.CreateResult = CreateResult.Success;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentService, string.Format("Get expire authorize charges for date: {0} fail. Ex: {1}.", date, ex));
            }
            return result;
        }
        public Result<bool> ReAuthorize(Charge charge)
        {
            var result = new Result<bool>(CreateResult.Error, false);
            try
            {
                Uow.Wrap(u =>
                {
                    //var trans = _repPaymentTransaction.Table.FirstOrDefault();
                    var transWithGood = (from tr in _repPaymentTransaction.Table
                                         join gr in _repPaymentTransaction.TableFor<GoodRequest>() on tr.GoodRequestId equals gr.Id
                                         join gb in _repPaymentTransaction.TableFor<GoodBooking>() on gr.Id equals gb.GoodRequestId
                                         //join ug in _repPaymentTransaction.TableFor<UserGood>() on gr.GoodId equals ug.GoodId
                                         where tr.TransactionId == charge.Token && tr.StatusId == (int)PaymentTransactionStatus.Pending && tr.Type == PaymentTransactionType.AuthorizeType
                                         select new
                                         {
                                             Transaction = tr,
                                             Good = new
                                             {
                                                 GoodId = gr.GoodId,
                                                 UserId = gr.UserId,
                                                 StartDate = gb.StartDate,
                                                 EndDate = gb.EndDate,
                                                 StatusId = gr.StatusId
                                             }
                                         }).FirstOrDefault();
                    if (transWithGood != null)
                    {
                        var closeDay = DateTime.Now.AddHours(-24 + (-1 * (_goodRequestService.CloseHoursAfterEnd)));
                        if ((transWithGood.Good.StatusId == (int)UserRequestStatus.Paid ||
                            transWithGood.Good.StatusId == (int)UserRequestStatus.Released)
                            && transWithGood.Good.EndDate <= closeDay)
                        {
                            var r = Authorize(transWithGood.Transaction.Cost, charge.Description, charge.Email, charge.Card.Token, transWithGood.Transaction.GoodRequestId, transWithGood.Transaction.CreateBy, u);
                            transWithGood.Transaction.StatusId = (int)PaymentTransactionStatus.Close;
                            _repPaymentTransaction.SaveOrUpdateAudit(transWithGood.Transaction, transWithGood.Good.UserId);
                        }
                    }
                }, null, LogSource.PinPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.PinPaymentService, string.Format("ReAuthorize for prev charge {0} fail. Ex: {1}.", charge.Token, ex));
            }
            return result;
        }
        public Result<bool> Refund(int goodRequestId, int userId)
        {
            var result = new Result<bool>(CreateResult.Error, false);
            try
            {
                Uow.Wrap(u =>
                {
                    var transList = (from tr in _repPaymentTransaction.Table
                                     join gr in _repPaymentTransaction.TableFor<GoodRequest>() on tr.GoodRequestId equals gr.Id
                                     where tr.GoodRequestId == goodRequestId && tr.Type == PaymentTransactionType.SaleType
                                     && tr.StatusId == (int)PaymentTransactionStatus.Pending
                                     && gr.StatusId == (int)UserRequestStatus.Paid
                                     select tr).ToList();
                    var trans = transList.FirstOrDefault();
                    if (trans != null)
                    {
                        RefundResponse refund = null;
                        try
                        {
                            refund = _pinService.Refund(trans.TransactionId, (int)(trans.Cost * 100));
                        }
                        catch (PinException ex)
                        {
                            Ioc.Get<IDbLogger>().LogError(LogSource.PinPaymentService, string.Format("Refund for charge {0} fail. Ex: {1}.", trans.TransactionId, ex));
                        }
                        if (refund != null && refund.Response != null)
                        {
                            trans.StatusId = (int)PaymentTransactionStatus.Close;
                            _repPaymentTransaction.SaveOrUpdateAudit(trans, userId);
                            var refTransaction = new PaymentTransaction()
                            {
                                StatusId = (int)PaymentTransactionStatus.Pending,
                                Commision = 0,
                                Cost = trans.Cost,
                                GoodRequestId = goodRequestId,
                                TransactionId = refund.Response.Token,
                                Type = PaymentTransactionType.Refund,
                                PayerId = refund.Response.Charge
                            };
                            _repPaymentTransaction.SaveOrUpdateAudit(refTransaction, userId);
                        }
                    }
                }, null, LogSource.PinPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.PinPaymentService, string.Format("Refund for goodRequest {0} fail. Ex: {1}.", goodRequestId, ex));
            }
            return result;
        }
    }
}
