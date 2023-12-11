using System;
using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Core.Services;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.Entities.Entities;
using Momentarily.ViewModels.Models;
namespace Momentarily.UI.Service.Services
{
    public interface IMomentarilyItemDataService : IGoodService<MomentarilyItem, MomentarilyItemMapViewModel>, IDependency
    {
        List<ListMomentarilyItemViewModel> GetUsersItems(int userId);
        Result<MomentarilyItem> SaveUserItem(MomentarilyItem item, int userId);
        Result<MomentarilyItem> GetMyItem(int userId, int itemId);
        Result<FilteredGoodsModel> GetFilteredItems(MomentarilyItemSearchModel searchModel);
        Result<MomentarilyItemMapViewModel> GetItem(int itemId);
        bool ArchiveGood(int itemId, int userId);
        List<MomentarilyItem> GetAvalableItem();
        List<MostRentedItems> GetTotalLoanedItemList();
        List<MostRentedItems> GetMostRentedItem();
        List<MostRentedItems> GetTotalLoanedItemListForAdmin();
        List<MomentarilyItem> GetNewItemsList();

        int GetTotalLoanedItem();
        bool DeleteArchiveGood(int itemId);
        UserDashboardViewModel GetTotalLoanedItemList(int? _userID);
        List<MostRankingCategory> GetTopRankingofCategory();
        List<Categories> categoriesList();
        List<MostRentedItems> gettotalborrowerslistbyUser(int? userId);
        List<MostRentedItems> TotalUserEarning(int? userId);
        List<MostRentedItems> TotalUserSpent(int? userId);
        List<MostRentedItems> TotalPendingAmount(int? userId);

        List<MostRentedItems> TotalUserEarningByMonth(int? userId);
        List<MostRentedItems> TotalUserSpentByMonth(int? userId);


        List<ProductsList> GetProductList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId);
        List<sharers> GetUserList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId);
        List<CategoriesList> GetCategoriesList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId);
        List<Borrowers> GetBorrowerList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId);

        List<ProductsList> GetAllDataProductList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId);
        List<CategoriesList> GetAllDataCategoriesList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId);

        List<sharers> GetAllDataUserList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId);
        List<Borrowers> GetAllDataBorrowerList(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId);

        List<AllTransactionReports> getAllTransactionData();
        List<string> categoriesListbyGoodId(int goodid);
        List<AllTransactionReports> GetReportByCategory(List<AllTransactionReports> allTransactionReports, int CategoryId);
        int RatingToOwnerPerRequest(int requestid, int sharerid);
        int RatingToBorrowerPerRequest(int requestid, int seekerId);

        string getUserEmail(int userID);

        List<CouponType> getCouponList();

        bool SaveUserCoupon(UserCouponVM userCouponVM);
        bool UpdateUserCoupon(UserCouponVM userCouponVM);

        List<UserCouponVM> getAllCouponList();

        UserCouponVM getEditCouponDetail(int? id);

        bool DeleteCoupon(int Id);

        int blockedCoupon(int CouponId, bool checkedValue);
        bool AlreadyExsistByCouponType(int CouponType);

        Result<PriceViewModel> CalculateCouponAmount(string CouponCode, double CustomerCost, int GoodId, string StartDate, string EndDate, double ShippingDistance, bool ApplyForDelivery,int currentuservalue);

        List<FAQList> GetFAQs();

        List<RecentlyRentedProduct> GetMostRecentlyRentedProduct();
        List<RecentlyRentedProduct> GetMostFeaturedProduct();
        List<ChatTopicsVM> GetChatTopics(bool status);

        bool getUserverificationForChatBot(string email,string phone);
        List<ChatBotQuestionsVM> GetChatQuestions(int topicId);
        List<ChatBotQuestionsŌrAnswersVM> GetChatQuestionsOrAnswers(int questionid);
        

    }
}