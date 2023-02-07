using System.Collections.Generic;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Entities.Entities;
using Apeek.Entities.Web;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.ViewModels.Models;

namespace Apeek.Core.Services
{
    public interface IUserDataService<T> : IDependency
    {
        Result<UserPublicProfile> GetPublicUserProfile(int userId);
        Result<ListViewModel<ReviewViewModel>> GetSeekersReview(int userId, int page = 1);
        Result<ListViewModel<ReviewViewModel>> GetSharersReview(int userId, int page = 1);
        ListViewModel<ReviewViewModel> GetListReviews(IQueryable<GoodBookingRank> items, int currentPage = 1);
        int GetUserCount();
        int GetAvailableItemsCount();
        int GetNewLendersCount();
        int GetNewBorrowersCount();
        int GetTotalNewItemsCount();
        int GetTotalLendersCount();
        int GetTotalBorrowersCount();
        List<User> GetAllUser();
        List<Subscribes> GetAllSubscriber();
        int UserBlocked(int userId, bool checkedValue);

        //List<Good> GetAvailableItems();
        double GetTotalEarning();

        List<User> Getnewborrowerslist();        List<User> GetTotalborrowerslist();        List<User> GetnewLenderslist();        List<User> GetTotalLenderslist();

        List<SharedUsers> GetDLLLenderslist();        List<SharedUsers> GetDLLBorrowersList();
        string GetCountryCodeByPhoneNumber(string number);
        BookingRankViewModal GetUserReviews(int userid);

        bool GetupdateGoodBookingRank(int id);

    }
}