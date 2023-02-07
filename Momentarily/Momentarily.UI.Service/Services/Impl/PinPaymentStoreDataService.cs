using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Mappers;
using Apeek.ViewModels.Models.Impl;
using Momentarily.Entities.Entities;
using Momentarily.NH.Repository.Repositories;
using Momentarily.ViewModels.Mappers;
using Momentarily.ViewModels.Models;
using PinPayments.Models;
namespace Momentarily.UI.Service.Services.Impl
{
    public class PinPaymentStoreDataService : IPinPaymentStoreDataService
    {
        private readonly IRepositoryUserCard _repUserCard;
        private readonly IPinPaymentCustomerRepository _repPinPaymentCustomerRepository;
        private readonly IPinPaymentRecipientRepository _repPinPaymentrecipient;
        private readonly IMomentarilyAccountDataService _momentarilyAccountDataService;
        public PinPaymentStoreDataService(IRepositoryUserCard repUserCard, IMomentarilyAccountDataService momentarilyAccountDataService, IPinPaymentCustomerRepository repPinPaymentCustomerRepository, IPinPaymentRecipientRepository repPinPaymentrecipient)
        {
            _repUserCard = repUserCard;
            _momentarilyAccountDataService = momentarilyAccountDataService;
            _repPinPaymentCustomerRepository = repPinPaymentCustomerRepository;
            _repPinPaymentrecipient = repPinPaymentrecipient;
        }
        public Result<PinPaymentCard> AddUserCard(int userId, PinPaymentCardViewModel cardViewModel)
        {
            var result = new Result<PinPaymentCard>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var card = new PinPaymentCard();
                    card = Ioc.Get<IPinPaymentCardMapper>().Map(cardViewModel, card);
                    card.UserId = userId;
                    result.Obj = _repUserCard.SaveOrUpdateAudit(card, userId);
                    result.CreateResult = CreateResult.Success;
                }, null, LogSource.BankCardDataService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentStoreDataService, string.Format("Add card fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<List<PinPaymentCardViewModel>> GetUserCards(int userId)
        {
            var result = new Result<List<PinPaymentCardViewModel>>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var mapper = Ioc.Get<IPinPaymentCardMapper>();
                    var cards = _repUserCard.GetUserCards(userId).ToList();
                    result.Obj = cards.Select(c => mapper.Map(c, new PinPaymentCardViewModel())).ToList();
                    result.CreateResult = CreateResult.Success;
                }, null, LogSource.BankCardDataService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentStoreDataService, string.Format("Get user cards fail. Ex: {0}.", ex));
            }
            return result;
        }
        public Result<PinPaymentCustomer> GetPinCustomer(int userId)
        {
            var result = new Result<PinPaymentCustomer>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var pinUser = _repPinPaymentCustomerRepository.Get(userId);
                    if (pinUser != null)
                    {
                        result.Obj = pinUser;
                    }
                    result.CreateResult = CreateResult.Success;
                }, null, LogSource.PinPaymentService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentStoreDataService, string.Format("Get customer with id: {0} fail. Ex: {1}.", userId, ex));
            }
            return result;
        }
        public Result<PinPaymentCustomer> AddCustomer(int userId, string customerToken)
        {
            var result = new Result<PinPaymentCustomer>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var customer = _repPinPaymentCustomerRepository.SaveOrUpdateAudit(new PinPaymentCustomer { Id = userId, CustomerToken = customerToken }, userId);
                    if (customer != null)
                    {
                        result.Obj = customer;
                        result.CreateResult = CreateResult.Success;
                    }
                }, null, LogSource.PinPaymentStoreDataService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentStoreDataService, string.Format("Get customer with id: {0} fail. Ex: {1}.", userId, ex));
            }
            return result;
        }
        public Result<string> GetRecipientsToken(int userId)
        {
            var result = new Result<string>(CreateResult.Error, null);
            try
            {
                Uow.Wrap(u =>
                {
                    var r = _repPinPaymentrecipient.Get(userId);
                    if (r != null) result.Obj = r.RecipientToken;
                    result.CreateResult = CreateResult.Success;
                }, null, LogSource.PinPaymentStoreDataService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentStoreDataService, string.Format("Get user bank info for user id: {0} fail. Ex: {1}.", userId, ex));
            }
            return result;
        }
        public Result<bool> AddRecipient(int userId, string recipientToken)
        {
            var result = new Result<bool>(CreateResult.Error, false);
            try
            {
                Uow.Wrap(u =>
                {
                    var rec = new PinPaymentRecipient() { Id = userId, RecipientToken = recipientToken };
                    _repPinPaymentrecipient.SaveOrUpdateAudit(rec, userId);
                    result.CreateResult = CreateResult.Success;
                    result.Obj = true;
                }, null, LogSource.PinPaymentStoreDataService);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.PinPaymentStoreDataService, string.Format("Save recipient for user id: {0} with token: {1} fail. Ex: {2}.", userId, recipientToken, ex));
            }
            return result;
        }
    }
}