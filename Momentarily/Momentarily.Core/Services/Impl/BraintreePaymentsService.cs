using BraintreePayments;
using System;
using System.Linq;
using BraintreePayments.Models;
using Apeek.Common.Models;
using Momentarily.NH.Repository.Repositories.Impl;
using Momentarily.Entities.Entities;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories.Impl;
using Apeek.Common.Definitions;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services;
using Momentarily.UI.Service.Services;
using System.Globalization;
using Apeek.ViewModels.Models.Impl;
namespace Momentarily.Core.Services.Impl
{
    public class BraintreePaymentsService
    {
        private BraintreeAPI _braintreeAPI;
        private BraintreeRecipientRepository _braintreeRecipientRepository;
        private RepositoryUser _repositoryUser;
        private RepositoryGoodRequest _repositoryGoodRequest;
        private RepositoryUserGood _repositoryUserGood;
        private ISettingsDataService _settingsService;
        private IMomentarilyGoodRequestService _goodRequestService;
        private IMomentarilyItemDataService _goodDataService;
        private IMomentarilyUserMessageService _userMessageService;
        private ISendMessageService _sendMessageService;
        private IAccountDataService _accountDataService;
        private readonly ISettingsDataService _settingsDataService;
        public BraintreePaymentsService()
        {
            _braintreeRecipientRepository = new BraintreeRecipientRepository();
            _repositoryUser = new RepositoryUser();
            _repositoryGoodRequest = new RepositoryGoodRequest();
            _repositoryUserGood = new RepositoryUserGood();
            _settingsService = Ioc.Get<ISettingsDataService>();
            _goodRequestService = Ioc.Get<IMomentarilyGoodRequestService>();
            _goodDataService = Ioc.Get<IMomentarilyItemDataService>();
            _userMessageService = Ioc.Get<IMomentarilyUserMessageService>();
            _sendMessageService = Ioc.Get<ISendMessageService>();
            _accountDataService = Ioc.Get<IAccountDataService>();
            _settingsDataService = Ioc.Get<ISettingsDataService>();
            _braintreeAPI = new BraintreeAPI();
            _braintreeAPI.Init(
                Environment: _settingsDataService.GetBraintreeEnvironment(),
                MasterMerchantAccountId: _settingsDataService.GetBraintreeMasterMerchantAccountId(),
                MerchantId: _settingsDataService.GetBraintreeMerchantId(),
                PublicKey: _settingsDataService.GetBraintreePublicKey(),
                PrivateKey: _settingsDataService.GetBraintreePrivateKey()
            );
        }
        public Result<ClientToken> GetClientToken()
        {
            var result = new Result<ClientToken>(CreateResult.Error, null);
            var getClientResult = _braintreeAPI.GetClientToken();
            if (getClientResult.IsSuccess)
            {
                result.Obj = getClientResult.ClientToken;
                result.CreateResult = CreateResult.Success;
            }
            result.Message = getClientResult.Message;            
            return result;
        }
        public Result<Object> CreateOrUpdateRecipient(int UserId, MerchantAccountIndividual Individual, MerchantAccountFunding Funding)
        {           
            var result = new Result<Object>(CreateResult.Error, null);
            try
            {                
                Uow.Wrap(u =>
                {
                    var recipient = _braintreeRecipientRepository.Table.Where(x => x.UserId == UserId).FirstOrDefault();                    
                    var createOrUpdateRecipientResult = _braintreeAPI.CreateOrUpdateMerchantAccount(
                        MerchantAccountId: recipient != null ? recipient.MerchantAccountId : null,
                        Individual: Individual,
                        Funding: Funding
                    );
                    if (createOrUpdateRecipientResult.IsSuccess)
                    {
                        if (recipient == null)
                        {
                            _braintreeRecipientRepository.SaveAudit(
                                entity: new BraintreeRecipient
                                {
                                    UserId = UserId,
                                    MerchantAccountId = createOrUpdateRecipientResult.MerchantAccount.Id
                                },
                                userId: UserId
                            );
                        }
                        else
                        {
                            _braintreeRecipientRepository.UpdateAudit(
                                entity: recipient,
                                userId: UserId
                            );
                        }
                        result.CreateResult = CreateResult.Success;                        
                    }
                    result.Message = createOrUpdateRecipientResult.Message;
                }, null, LogSource.BraintreePaymentsService);
            }
            catch (Exception ex)
            {
                result.Message = "Fail to create or update recipient.";
                Ioc.Get<IDbLogger>().LogException(LogSource.BraintreePaymentsService, ex);
            }
            return result;
        }       
        public Result<MerchantAccount> GetRecipientByUserId(int UserId)
        {
            var result = new Result<MerchantAccount>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var recipient = _braintreeRecipientRepository.Table.Where(x => x.UserId == UserId).FirstOrDefault();
                    if (recipient != null)
                    {
                        var getMerchantAccountResult = _braintreeAPI.GetMerchantAccountById(recipient.MerchantAccountId);
                        if (getMerchantAccountResult.IsSuccess)
                        {
                            result.Obj = getMerchantAccountResult.MerchantAccount;
                            result.CreateResult = CreateResult.Success;
                        }
                        result.Message = getMerchantAccountResult.Message;
                    }
                }, null, LogSource.BraintreePaymentsService);
            }
            catch (Exception ex)
            {
                result.Message = "Fail to get recipient by user id";
                Ioc.Get<IDbLogger>().LogException(LogSource.BraintreePaymentsService, ex);
            }
            return result;
        }
        public Result<Object> Pay(int UserId, int GoodRequestId, string PaymentMethodNonce)
        {
            var result = new Result<Object>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var user = _repositoryUser.GetUser(UserId);
                    var goodRequest = _repositoryGoodRequest.Get(GoodRequestId);                    
                    if (goodRequest.StatusId == (int)UserRequestStatus.Approved)
                    {
                        var userGood = _repositoryUserGood.Table.Where(x => x.GoodId == goodRequest.GoodId).First();
                        var recipient = _braintreeRecipientRepository.Table.Where(x => x.UserId == userGood.UserId).First();                        
                        var createOrUpdateCustomerResult = _braintreeAPI.CreateOrUpdateCustomer(
                                    CustomerId: user.Id.ToString(),
                                    FirstName: user.FirstName,
                                    LastName: user.LastName,
                                    Email: user.Email,
                                    PaymentMethodNonce: PaymentMethodNonce
                                 );
                        if (createOrUpdateCustomerResult.IsSuccess)
                        {
                            var amount = (decimal)goodRequest.CustomerCost;
                            var serviceFeeAmount = (decimal)(goodRequest.CustomerServiceFeeCost + goodRequest.CustomerCharityCost + goodRequest.SharerServiceFeeCost + goodRequest.SharerCharityCost);
                            var createPurchase = _braintreeAPI.CreatePurchaseByToken(
                                                    Token: createOrUpdateCustomerResult.Customer.PaymentMethodToken,
                                                    Amount: amount,
                                                    ServiceFeeAmount: serviceFeeAmount,
                                                    MerchantAccountId: recipient.MerchantAccountId);
                            if (createPurchase.IsSuccess)
                            {                                
                                result.CreateResult = CreateResult.Success;
                            } else
                            {
                                result.Message = createPurchase.Message;
                                result.CreateResult = CreateResult.Error;
                            }
                        }
                        else
                        {
                            result.Message = createOrUpdateCustomerResult.Message;
                        }
                    } else
                    {
                        result.Message = "Good request status differs from 'approved'";
                        result.CreateResult = CreateResult.Error;
                    }
                }, null, LogSource.BraintreePaymentsService);
            }
            catch (Exception ex)
            {
                result.Message = "Fail to pay.";
                Ioc.Get<IDbLogger>().LogException(LogSource.BraintreePaymentsService, ex);
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
                if (seekerUserInfo != null && sharerUserInfo != null)
                {                    
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
                        quickUrl.UserRequestAbsoluteUrl(goodRequestId));
                }
            }
            catch (Exception ex)
            {                             
                Ioc.Get<IDbLogger>().LogException(LogSource.BraintreePaymentsService, string.Format("Send emails for request: {0} fail. Ex: {1}.", goodRequestId, ex), ex);
            }
        }
        public Result<Object> RefundSecurityDeposit(int GoodRequestId, int userId)
        {
            var result = new Result<Object>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {                    
                    var goodRequest = _repositoryGoodRequest.Get(GoodRequestId);
                    if (goodRequest.StatusId != (int)UserRequestStatus.Refunded)
                    {
                        var userGood = _repositoryUserGood.Table.Where(x => x.GoodId == goodRequest.GoodId).First();
                        var recipient = _braintreeRecipientRepository.Table.Where(x => x.UserId == userGood.UserId).First();                        
                        var createOrUpdateCustomerResult = _braintreeAPI.GetCustomer(CustomerId: goodRequest.UserId.ToString());                                    
                        if (createOrUpdateCustomerResult.IsSuccess)
                        {                                           
                            var createPurchase = _braintreeAPI.CreatePurchaseByToken(
                                                    Token: createOrUpdateCustomerResult.Customer.PaymentMethodToken,
                                                    Amount: (decimal)goodRequest.SecurityDeposit,
                                                    ServiceFeeAmount: 0,
                                                    MerchantAccountId: recipient.MerchantAccountId);
                            if (createPurchase.IsSuccess)
                            {
                                result.CreateResult = CreateResult.Success;
                                goodRequest.StatusId = (int)UserRequestStatus.Refunded;
                                _repositoryGoodRequest.SaveOrUpdateAudit(goodRequest, userId);
                            }
                            else
                            {
                                result.Message = createPurchase.Message;
                                result.CreateResult = CreateResult.Error;
                            }
                        }
                        else
                        {
                            result.Message = createOrUpdateCustomerResult.Message;
                        }
                    }
                }, null, LogSource.BraintreePaymentsService);
            }
            catch (Exception ex)
            {
                result.Message = "Fail to refund security deposit.";
                Ioc.Get<IDbLogger>().LogException(LogSource.BraintreePaymentsService, ex);
            }
            return result;
        }
    }
}
