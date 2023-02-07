using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models.Impl;
using Momentarily.Entities.Entities;
namespace Momentarily.UI.Service.Services
{
    public interface IPinPaymentStoreDataService:IDependency
    {
        Result<PinPaymentCard> AddUserCard(int userId, PinPaymentCardViewModel cardViewModel);
        Result<List<PinPaymentCardViewModel>> GetUserCards(int userId);
        Result<PinPaymentCustomer> GetPinCustomer(int userId);
        Result<PinPaymentCustomer> AddCustomer(int userId, string customerToken);
        Result<string> GetRecipientsToken(int userId);
        Result<bool> AddRecipient(int userId, string recipientToken);
    }
}