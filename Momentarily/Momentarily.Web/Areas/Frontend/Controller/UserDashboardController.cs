using Apeek.Common;
using Apeek.Core.Services;
using Apeek.ViewModels.Models.Impl;
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
        private readonly IMomentarilyItemDataService _momentarilyItemDataService;
        private readonly IUserDataService<MomentarilyItem> _userService;
        public UserDashboardController(IMomentarilyItemDataService momentarilyItemDataService)
        {
            _momentarilyItemDataService = momentarilyItemDataService;
            _userService = Ioc.Get<IUserDataService<MomentarilyItem>>();
        }


        // GET: Frontend/UserDashboard
        [AllowAnonymous]
        public ActionResult Index()
        {
            double totalUserEarning = 0;
            int? _userID = UserId;
            UserDashboardViewModel obj = new UserDashboardViewModel();
            obj = _momentarilyItemDataService.GetTotalLoanedItemList(_userID);
            obj.totalBorrowersListbyUser = _momentarilyItemDataService.gettotalborrowerslistbyUser(_userID);
            obj.totalBorrowersCountbyUser = obj.totalBorrowersListbyUser.Sum(x => x.Count);

            obj.totalUserEarningListbyUser = _momentarilyItemDataService.TotalUserEarning(_userID);
            obj.totalUserEarning = obj.totalUserEarningListbyUser.Sum(x => x.Total);
            //var dt = Math.Round(totalUserEarning, 2);
            //obj.totalUserEarning = Math.Round(totalUserEarning, 2);
            obj.totalUserSpendListbyUser = _momentarilyItemDataService.TotalUserSpent(_userID);
            obj.totalUserSpendListbyUser = _momentarilyItemDataService.TotalUserSpent(_userID);
            obj.totalUserSpend = obj.totalUserSpendListbyUser.Sum(x => x.Total);
            obj.totalUserEarningByMonth = _momentarilyItemDataService.TotalUserEarningByMonth(_userID);
            obj.totalUserSpendByMonth = _momentarilyItemDataService.TotalUserSpentByMonth(_userID);



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