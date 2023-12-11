using Apeek.Common;
using Apeek.Core.Services;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Models.Impl;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Web.Framework.Controllers;
using Momentarily.Entities.Entities;
using Momentarily.UI.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class UserDashboardController : FrontendController
    {
        private readonly UserControllerHelper _helper;
        private readonly IMomentarilyItemDataService _momentarilyItemDataService;
        private readonly IUserDataService<MomentarilyItem> _userService;
        private readonly IRepositoryGoodBookingRank _repBookingRank;
        public UserDashboardController(IMomentarilyItemDataService momentarilyItemDataService, IRepositoryGoodBookingRank repBookingRank)
        {
            _helper = new UserControllerHelper();
            _momentarilyItemDataService = momentarilyItemDataService;
            _userService = Ioc.Get<IUserDataService<MomentarilyItem>>();
            _repBookingRank = repBookingRank;
        }


        //[AllowAnonymous]
        //public ActionResult Index()
        //{
        //    int? userId = UserId;  // Ensure that UserId is set correctly in your code
        //    if (userId.HasValue)
        //    {
        //        UserDashboardViewModel viewModel = new UserDashboardViewModel();

        //        // Populate mostloanedItems
        //        // viewModel.ReviewCount = /* logic to get review count */;
        //        // viewModel.Rank = /* logic to get user rank */;
        //        viewModel.SharersReviews = (ReviewViewModel)_repBookingRank.GetRankFromSharers(userId);
        //        viewModel.SeekersReviews = (ReviewViewModel)_repBookingRank.GetRankFromSeekers(userId);


        //        //viewModel.mostloanedItems = _momentarilyItemDataService.GetTotalLoanedItemList(_userID);
        //        viewModel = _momentarilyItemDataService.GetTotalLoanedItemList(userId.Value);
        //        viewModel.mostloanedItemsCount = viewModel.mostloanedItems.Count;

        //        // Populate totalBorrowersListbyUser and totalBorrowersCountbyUser
        //        viewModel.totalBorrowersListbyUser = _momentarilyItemDataService.gettotalborrowerslistbyUser(userId.Value);
        //        viewModel.totalBorrowersCountbyUser = viewModel.totalBorrowersListbyUser.Sum(x => x.Count);

        //        // Populate totalUserEarning and totalUserEarningListbyUser
        //        viewModel.totalUserEarningListbyUser = _momentarilyItemDataService.TotalUserEarning(userId.Value);
        //        viewModel.totalUserEarning = viewModel.totalUserEarningListbyUser.Sum(x => x.Total);

        //        // Populate totalUserSpend and totalUserSpendListbyUser
        //        viewModel.totalUserSpendListbyUser = _momentarilyItemDataService.TotalUserSpent(userId.Value);
        //        viewModel.totalUserSpend = viewModel.totalUserSpendListbyUser.Sum(x => x.Total);

        //        // Populate totalUserEarningByMonth
        //        viewModel.totalUserEarningByMonth = _momentarilyItemDataService.TotalUserEarningByMonth(userId.Value);

        //        // Populate totalUserSpendByMonth
        //        viewModel.totalUserSpendByMonth = _momentarilyItemDataService.TotalUserSpentByMonth(userId.Value);

        //        return View(viewModel);
        //    }
        //    else
        //    {
        //        // Handle the case where UserId is not set
        //        // You might want to redirect to a login page or handle it accordingly
        //        return RedirectToAction("Login", "Account");  // Adjust this accordingly
        //    }
        //}


        // GET: Frontend/UserDashboard
        [AllowAnonymous]
        public ActionResult Index()
        {
            double totalUserEarning = 0;
            int? _userID = UserId;
            UserDashboardViewModel obj = new UserDashboardViewModel();
            var result = _userService.GetPublicUserProfile(_userID.Value);
            if (result.CreateResult == CreateResult.Success)
            {
                // Assign relevant properties from the result to the viewModel


                // Add other properties as needed
                obj = _momentarilyItemDataService.GetTotalLoanedItemList(_userID);
                obj.totalBorrowersListbyUser = _momentarilyItemDataService.gettotalborrowerslistbyUser(_userID);
                obj.totalBorrowersCountbyUser = obj.totalBorrowersListbyUser.Sum(x => x.Count);
                obj.totalUserPendingAmount = _momentarilyItemDataService.TotalPendingAmount(_userID);

                obj.totalUserEarningListbyUser = _momentarilyItemDataService.TotalUserEarning(_userID);
                obj.totalUserEarning = obj.totalUserEarningListbyUser.Sum(x => x.Total); // Summing up the Total property
                //var dt = Math.Round(totalUserEarning, 2);
                //obj.totalUserEarning = Math.Round(totalUserEarning, 2);
                obj.totalUserSpendListbyUser = _momentarilyItemDataService.TotalUserSpent(_userID);
                obj.totalUserSpend = obj.totalUserSpendListbyUser.Sum(x => x.Total);
                obj.totalUserEarningByMonth = _momentarilyItemDataService.TotalUserEarningByMonth(_userID);
                obj.totalUserSpendByMonth = _momentarilyItemDataService.TotalUserSpentByMonth(_userID);

                obj.RankSeekers = (int)result.Obj.Rank;
                obj.ReviewCount = result.Obj.ReviewCount; 
                obj.TotalRentals = result.Obj.TotalRentals; 
                obj.TotalCancelledRentals = result.Obj.TotalCancelledRentals;
                var name = result.Obj.User.FirstName + " " + result.Obj.User.LastName;
                obj.Name = name;//result.Obj.User.FirstName;
                obj.Description = result.Obj.User.Description;
                obj.RankSharers = result.Obj.RankSharers;
                return View(obj);
           
            }
         


            return View(obj);
        }
        //public ActionResult LoanedItemsList()
        //{
        //    int? _userID = UserId;
        //    List<MostRentedItems> list =_momentarilyItemDataService.GetTotalLoanedItemList(_userID);
        //    return View();
        //}
    }
}