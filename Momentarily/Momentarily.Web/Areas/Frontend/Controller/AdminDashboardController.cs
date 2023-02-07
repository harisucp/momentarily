using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Apeek.Web.Framework;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Web.Framework.Controllers;
using Momentarily.Entities.Entities;
using Momentarily.UI.Service.Services;
using Momentarily.ViewModels.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PayPal.Api;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Data.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using static Apeek.ViewModels.Models.Impl.SubscriberViewModel;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    [RedirectNotIsAdminFilterAttribute]
    public class AdminDashboardController : FrontendController
    {
        // GET: Frontend/AdminDashboard
        private readonly AdminControllerHelper _helper;
        private readonly MessageControllerHelper _Messagehelper;
        private readonly IUserDataService<MomentarilyItem> _userService;
        private readonly IMomentarilyItemDataService _momentarilyItemDataService;
        private readonly IAccountDataService _accountDataService;
        private readonly IMomentarilyItemTypeService _typeService;
        private readonly IGoodRequestService _goodRequestService;

        private readonly IUserMessageService _userMessageService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly ISendMessageService _sendMessageService;
        private readonly IUserLogService _userLogService;
        private readonly AccountControllerHelper<MomentarilyRegisterModel> _helperAccount;
        static string key { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";
        // private readonly IMomentarilyItemDataService _itemDataService;
        private readonly IHealthDataService _healthDataService;
        private readonly StockControllerHelper _Stockhelper;
        public AdminDashboardController(IMomentarilyItemDataService momentarilyItemDataService,
            IAccountDataService accountDataService,
            IMomentarilyItemTypeService typeService,
             IUserMessageService userMessageService,
        IUserNotificationService userNotificationService,
        ISendMessageService sendMessageService,
        StockControllerHelper stockhelper,
        IUserLogService userLogService, IHealthDataService healthDataService)
        {
            _helper = new AdminControllerHelper();
            _Messagehelper = new MessageControllerHelper();
            _userService = Ioc.Get<IUserDataService<MomentarilyItem>>();
            _momentarilyItemDataService = momentarilyItemDataService;
            _accountDataService = accountDataService;
            _typeService = typeService;
            _userMessageService = userMessageService;
            _userMessageService = userMessageService;
            _sendMessageService = sendMessageService;
            _helperAccount = new AccountControllerHelper<MomentarilyRegisterModel>();
            _userLogService = userLogService;
            _healthDataService = healthDataService;
            _Stockhelper = stockhelper;
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
         
            int _userCount = _userService.GetUserCount();
            ViewBag.usercount = _userCount;
            int _availableitemsCount = _userService.GetAvailableItemsCount();
            ViewBag.availableitemsCount = _availableitemsCount;
            int _newlendersCount = _userService.GetNewLendersCount();
            ViewBag.newlendersCount = _newlendersCount;

            int _newBorrowersCount = _userService.GetNewBorrowersCount();
            ViewBag.newBorrowersCount = _newBorrowersCount;

            int _newtotalitemsCount = _userService.GetTotalNewItemsCount();
            ViewBag.newtotalitemsCount = _newtotalitemsCount;

            int _totallenderCount = _userService.GetTotalLendersCount();
            ViewBag.totallenderCount = _totallenderCount;

            int _totalBorrowersCount = _userService.GetTotalBorrowersCount();
            ViewBag.totalBorrowersCount = _totalBorrowersCount;

            double _totalEarning = _userService.GetTotalEarning();
            ViewBag.totalEarning = _totalEarning;

            int totalLoanedItemsCount = _momentarilyItemDataService.GetTotalLoanedItem();
            ViewBag.totalLoanedItems = totalLoanedItemsCount;

            var newItemsList = _momentarilyItemDataService.GetNewItemsList();
            TempData["newItemsList"] = newItemsList;
            MostRentedItems obj = new MostRentedItems();
            obj.mostRentedItems = _momentarilyItemDataService.GetMostRentedItem();
            obj.totalLoanedItems = _momentarilyItemDataService.GetTotalLoanedItemListForAdmin();
            obj.topRankingofCategory = _momentarilyItemDataService.GetTopRankingofCategory();

            var totalSubscribers = _userService.GetAllSubscriber().GroupBy(x => x.Email).Select(y => y.First()).ToList();
            var totalSubscribersNew = totalSubscribers.Count();
            ViewBag.totalSubscribersCount = totalSubscribersNew;
            var todaySubscribers = _userService.GetAllSubscriber().Where(x => x.CreateDate.Date == DateTime.Now.Date).GroupBy(x => x.Email).Select(y => y.First()).ToList();
            var todaySubscribersNew = todaySubscribers.Count();
            ViewBag.todaySubscribersCount = todaySubscribersNew;

            ViewBag.totalSignUpCount = _userService.GetAllUser().Where(x => x.CreatedDate == null ? false : ((DateTime)x.CreatedDate).Date == DateTime.Now.Date).Count();


            return View(obj);
        }
        [AllowAnonymous]
        public ActionResult Users(string status)
        {
       
            UserListing obj = new UserListing();

            if (status == "All" || status == null)
            {
                obj.users = _userService.GetAllUser().ToList();
            }
            else
            {
                obj.users = _userService.GetAllUser().Where(x => x.CreatedDate == null ? false : ((DateTime)x.CreatedDate).Date == DateTime.Now.Date).ToList();
            }
            obj.Status = status == null ? "All" : status;
            return View(obj);
        }


        [HttpPost]
        public ActionResult ExportToCsvUsers(string status)        {
            UserListing obj = new UserListing();

            if (status == "All" || status == null)
            {
                obj.users = _userService.GetAllUser().ToList();
            }
            else
            {
                obj.users = _userService.GetAllUser().Where(x => x.CreatedDate == null ? false : ((DateTime)x.CreatedDate).Date == DateTime.Now.Date).ToList();
            }

            string basedOnStatusSheetName = status == "All" ? "All Users" : "Daily Users";

            ExcelPackage excelPackage = new ExcelPackage();            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(basedOnStatusSheetName);            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#666666");            Color colFromText = System.Drawing.ColorTranslator.FromHtml("#d8d6d1");            Color setColorborder = System.Drawing.ColorTranslator.FromHtml("#e9ecef");

            worksheet.TabColor = System.Drawing.Color.Wheat;
            worksheet.DefaultRowHeight = 12;
            worksheet.DefaultColWidth = 8;
            worksheet.Row(1).Height = 20;

            using (ExcelRange rng = worksheet.Cells["A1:F1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = basedOnStatusSheetName;
                rng.AutoFitColumns();
            }

            worksheet.Row(2).Height = 20;
            worksheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Row(2).Style.Font.Bold = true;
            worksheet.Row(2).Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[2, 1].Value = "User Id";            worksheet.Cells[2, 2].Value = "User Name";            worksheet.Cells[2, 3].Value = "Email Id";            worksheet.Cells[2, 4].Value = "Date of birth";
            worksheet.Cells[2, 5].Value = "Created On";            worksheet.Cells[2, 6].Value = "Blocked/Unblocked";            int srnoCount = 1;            for (int j = 0; j < obj.users.Count(); j++)            {                worksheet.Row(j + 3).Height = 20;                worksheet.Row(j + 3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;                worksheet.Row(j + 3).Style.Font.Color.SetColor(Color.Black);                worksheet.Cells[j + 3, 1].Value = obj.users[j].Id;
                worksheet.Cells[j + 3, 2].Value = obj.users[j].FullName;                worksheet.Cells[j + 3, 3].Value = obj.users[j].Email;                worksheet.Cells[j + 3, 4].Value = obj.users[j].DateOfBirth.Value.ToString("dd/MM/yyyy");
                worksheet.Cells[j + 3, 5].Value = obj.users[j].CreatedDate.HasValue ? obj.users[j].CreatedDate.Value.ToString("dd/MM/yyyy") : "";
                worksheet.Cells[j + 3, 6].Value = obj.users[j].IsBlocked == true ? "Yes" : "No";
                srnoCount++;            }            worksheet.Column(1).AutoFit();            worksheet.Column(2).AutoFit();            worksheet.Column(3).AutoFit();
            worksheet.Column(4).AutoFit();
            worksheet.Column(5).AutoFit();
            worksheet.Column(6).AutoFit();
            string excelName = basedOnStatusSheetName + " " + DateTime.Now.ToString("dd-MM-yyyy");            using (var memoryStream = new MemoryStream())            {                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");                excelPackage.SaveAs(memoryStream);                memoryStream.Position = 0;                TempData[excelName] = memoryStream.ToArray();            }            return new JsonResult()            {                Data = new { FileGuid = excelName, FileName = excelName + ".xlsx" }            };
        }


        [AllowAnonymous]
        public ActionResult Subscribers(string status)
        {
           

            SubscriberViewModel obj = new SubscriberViewModel();
            var subscribers = _userService.GetAllSubscriber();
            var subscriberNew = new List<SubscriberVM>();
            if (status == "All")
            {
                obj.subscribers = (from data in subscribers select new SubscriberVM { Id = data.Id, Email = data.Email, CreatedDate = data.CreateDate, SubscribeStatus = data.SubscribeStatus }).ToList();
            }
            else
            {
                obj.subscribers = (from data in subscribers where data.CreateDate.Date == DateTime.Now.Date select new SubscriberVM { Id = data.Id, Email = data.Email, CreatedDate = data.CreateDate, SubscribeStatus = data.SubscribeStatus }).ToList();
            }

            subscriberNew = obj.subscribers.GroupBy(x => x.Email).Select(y => y.First()).ToList();
            obj.subscribers = subscriberNew;
            obj.Status = status;
            return View(obj);
        }
        [HttpPost]
        public ActionResult ExportToCsvSubscribers(string status)        {
            SubscriberViewModel obj = new SubscriberViewModel();
            var subscribers = _userService.GetAllSubscriber();
            var subscriberNew = new List<SubscriberVM>();
            if (status == "All")
            {
                obj.subscribers = (from data in subscribers select new SubscriberVM { Id = data.Id, Email = data.Email, CreatedDate = data.CreateDate, SubscribeStatus = data.SubscribeStatus }).ToList();
            }
            else
            {
                obj.subscribers = (from data in subscribers where data.CreateDate.Date == DateTime.Now.Date select new SubscriberVM { Id = data.Id, Email = data.Email, CreatedDate = data.CreateDate, SubscribeStatus = data.SubscribeStatus }).ToList();
            }

            subscriberNew = obj.subscribers.GroupBy(x => x.Email).Select(y => y.First()).ToList();
            obj.subscribers = subscriberNew;

            string basedOnStatusSheetName = status == "All" ? "All Subscribers" : "Daily Subscribers";

            ExcelPackage excelPackage = new ExcelPackage();            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(basedOnStatusSheetName);            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#666666");            Color colFromText = System.Drawing.ColorTranslator.FromHtml("#d8d6d1");            Color setColorborder = System.Drawing.ColorTranslator.FromHtml("#e9ecef");

            worksheet.TabColor = System.Drawing.Color.Wheat;
            worksheet.DefaultRowHeight = 12;
            worksheet.DefaultColWidth = 8;
            worksheet.Row(1).Height = 20;

            using (ExcelRange rng = worksheet.Cells["A1:D1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = basedOnStatusSheetName;
                rng.AutoFitColumns();
            }

            worksheet.Row(2).Height = 20;
            worksheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Row(2).Style.Font.Bold = true;
            worksheet.Row(2).Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[2, 1].Value = "Sr.No #";            worksheet.Cells[2, 2].Value = "Subscriber";            worksheet.Cells[2, 3].Value = "Created On";
            worksheet.Cells[2, 4].Value = "Status";            int srnoCount = 1;            for (int j = 0; j < obj.subscribers.Count(); j++)            {                worksheet.Row(j + 3).Height = 20;                worksheet.Row(j + 3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;                worksheet.Row(j + 3).Style.Font.Color.SetColor(Color.Black);                worksheet.Cells[j + 3, 1].Value = srnoCount;                worksheet.Cells[j + 3, 2].Value = obj.subscribers[j].Email;                worksheet.Cells[j + 3, 3].Value = obj.subscribers[j].CreatedDate.Value.ToString("dd/MM/yyyy");
                worksheet.Cells[j + 3, 4].Value = obj.subscribers[j].SubscribeStatus == true ? "Subscribed" : "Unsubscribed";
                srnoCount++;            }            worksheet.Column(1).AutoFit();            worksheet.Column(2).AutoFit();            worksheet.Column(3).AutoFit();
            worksheet.Column(4).AutoFit();
            string excelName = basedOnStatusSheetName + " " + DateTime.Now.ToString("dd-MM-yyyy");            using (var memoryStream = new MemoryStream())            {                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");                excelPackage.SaveAs(memoryStream);                memoryStream.Position = 0;                TempData[excelName] = memoryStream.ToArray();            }            return new JsonResult()            {                Data = new { FileGuid = excelName, FileName = excelName + ".xlsx" }            };
        }

        [AllowAnonymous]        public ActionResult BlockedUser(int userId, bool checkedValue)        {                       int result = _userService.UserBlocked(userId, checkedValue);            bool sentmail = _accountDataService.UserBlockedMessage(userId, checkedValue);            return Json(result, JsonRequestBehavior.AllowGet);        }


        [AllowAnonymous]
        public ActionResult Items()
        {
            List<Momentarily.Entities.Entities.MomentarilyItem> list = new List<Momentarily.Entities.Entities.MomentarilyItem>();
       
            list = _momentarilyItemDataService.GetAvalableItem();
            return View(list);
        }

        [AllowAnonymous]
        public ActionResult NewBorrowersList()
        {
          
            UserListing obj = new UserListing();
            obj.users = _userService.Getnewborrowerslist();

            return View(obj);
        }

        [AllowAnonymous]
        public ActionResult TotalBorrowersList()
        {
       
            UserListing obj = new UserListing();
            obj.users = _userService.GetTotalborrowerslist();

            return View(obj);
        }

        [AllowAnonymous]
        public ActionResult NewLendersList()
        {
          
            UserListing obj = new UserListing();
            obj.users = _userService.GetnewLenderslist();

            return View(obj);
        }

        [AllowAnonymous]
        public ActionResult TotalLendersList()
        {
          
            UserListing obj = new UserListing();
            obj.users = _userService.GetTotalLenderslist();

            return View(obj);
        }
        [AllowAnonymous]
        public ActionResult TotalLoanedItems()
        {
            MostRentedItems obj = new MostRentedItems();
            
            obj.mostRentedItems = _momentarilyItemDataService.GetTotalLoanedItemList();
            return View(obj);
        }


        [AllowAnonymous]
        public ActionResult GetNewItemsList()
        {
            List<Momentarily.Entities.Entities.MomentarilyItem> list = new List<Momentarily.Entities.Entities.MomentarilyItem>();
           
            list = _momentarilyItemDataService.GetNewItemsList();
            return View(list);
        }

        [AllowAnonymous]
        public ActionResult MostRentedItems()
        {
            MostRentedItems obj = new MostRentedItems();
            List<MostRentedItems> list = new List<MostRentedItems>();
           
            obj.mostRentedItems = _momentarilyItemDataService.GetMostRentedItem();
            return View(obj);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AdminPwd()
        {
          
            AdminPwdModel obj = new AdminPwdModel();
            if (TempData["SetModel"] != null)
            {
                var data = TempData["SetModel"] as AdminPwdModel;
                obj.OldPassword = data.OldPassword;
                obj.NewPassword = data.NewPassword;
                obj.ConfirmPassword = data.ConfirmPassword;
            }

            //var shape = _shapeFactory.BuildShape(null, new AdminPwdModel(), PageName.AdminPwd.ToString());
            return View(obj);
        }

        [HttpPost]
        public ActionResult AdminPwd(AdminPwdModel shape)
        {
            //AdminPwdModel obj = new AdminPwdModel();
            TempData["SetModel"] = null;
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _accountDataService.SetAdminPwd(shape, UserId.Value);
                    if (result.CreateResult == CreateResult.OldPasswordDoesNotMatch)
                    {
                        TempData["SetModel"] = shape;
                        ModelState.AddModelError("OldPassword", "Old Password does not match with current password !");
                        return View(shape);
                    }
                    if (result.CreateResult == CreateResult.Success)
                    {
                        ViewBag.successmsg = ViewErrorText.AdminPwdChanged;
                        bool checkEmail = _helper.SendEmailChangePasswordMessage(result.Obj);
                        return View(new AdminPwdModel());
                    }
                    else
                    {
                        Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Update User Pwd: {0}", shape));
                        //shape.IsError = true;
                        ViewBag.errorMsg = ViewErrorText.AdminPwdNoChanged;
                        return View(shape);
                    }
                }
                return View(shape);
            }
            catch (Exception ex)
            {

            }
            return View(new AdminPwdModel());
        }

        public bool DeleteItem(int itemId)
        {
            var result = false;
            try
            {
                result = _momentarilyItemDataService.DeleteArchiveGood(itemId);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [AllowAnonymous]
        public ActionResult TotalListedItem()
        {
            List<Momentarily.Entities.Entities.MomentarilyItem> list = new List<Momentarily.Entities.Entities.MomentarilyItem>();
          
            list = _momentarilyItemDataService.GetNewItemsList();
            return View(list);
        }


        [AllowAnonymous]
        public ActionResult TransactionReport()
        {

            AdminReportingVM obj = new AdminReportingVM();
            var ddlCategories = _momentarilyItemDataService.categoriesList();
            var categoryList = new SelectList(ddlCategories.ToList(), "CategoryId", "CategoryName");
            ViewData["DDLCategories"] = categoryList;
            obj.StartRentalDate = DateTime.Now.AddMonths(-1);
            obj.EndRentalDate = DateTime.Now;
            obj.productsLists = _momentarilyItemDataService.GetProductList(DateTime.Now.AddMonths(-1), DateTime.Now.Date, "", "", 0, "", 0);
            obj.sharersLists = _momentarilyItemDataService.GetUserList(DateTime.Now.AddMonths(-1), DateTime.Now.Date, "", "", 0, "", 0);
            obj.categoriesList = _momentarilyItemDataService.GetCategoriesList(DateTime.Now.AddMonths(-1), DateTime.Now.Date, "", "", 0, "", 0);
            obj.borrowersList = _momentarilyItemDataService.GetBorrowerList(DateTime.Now.AddMonths(-1), DateTime.Now.Date, "", "", 0, "", 0);
            return View(obj);
        }

        [AllowAnonymous]

        public JsonResult TransactionReportPartial(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {

            var ddlCategories = _momentarilyItemDataService.categoriesList();
            var categoryList = new SelectList(ddlCategories.ToList(), "CategoryId", "CategoryName");
            ViewData["DDLCategories"] = categoryList;

            AdminReportingVM obj = new AdminReportingVM();
            obj.productsLists = _momentarilyItemDataService.GetProductList(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            obj.sharersLists = _momentarilyItemDataService.GetUserList(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            obj.categoriesList = _momentarilyItemDataService.GetCategoriesList(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            obj.borrowersList = _momentarilyItemDataService.GetBorrowerList(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public JsonResult GetTotal_DLL_Lenderslist(string Prefix)
        {
            var result = (dynamic)null;
            List<SharedUsers> _data = new List<SharedUsers>();
            _data = _userService.GetDLLLenderslist();
            result = (from N in _data
                      where N.Name.ToLower().StartsWith(Prefix.ToLower())
                      //where !string.IsNullOrEmpty(N.Name) ? N.Name.StartsWith(Prefix) : false
                      select new { N.Name });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetTotal_DLL_Borrowerslist(string Prefix)
        {
            var result = (dynamic)null;
            List<SharedUsers> _data = new List<SharedUsers>();
            _data = _userService.GetDLLBorrowersList();
            result = (from N in _data
                      where N.Name.ToLower().StartsWith(Prefix.ToLower())
                      select new { N.Name });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportToCsv(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)
        {
            AdminReportingVM obj = new AdminReportingVM();
            obj.productsListsAllData = _momentarilyItemDataService.GetAllDataProductList(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            obj.categoriesListAllData = _momentarilyItemDataService.GetAllDataCategoriesList(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            obj.sharersListsAllData = _momentarilyItemDataService.GetAllDataUserList(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            obj.borrowersListAllData = _momentarilyItemDataService.GetAllDataBorrowerList(startRentalDate, endRentalDate, searchShareName, searchBorrowerName, categoryId, ItemName, amountRangeId);
            var list = obj.productsListsAllData;
            var listcategories = obj.categoriesListAllData;
            var listSharers = obj.sharersListsAllData;
            var listborrowers = obj.borrowersListAllData;
            //Create a new ExcelPackage
            ExcelPackage excelPackage = new ExcelPackage();
            //Create the WorkSheet
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Product");
            ExcelWorksheet worksheet2 = excelPackage.Workbook.Worksheets.Add("Category");
            ExcelWorksheet worksheet3 = excelPackage.Workbook.Worksheets.Add("Sharer");
            ExcelWorksheet worksheet4 = excelPackage.Workbook.Worksheets.Add("Borrowers");


            worksheet.TabColor = System.Drawing.Color.Wheat;
            worksheet.DefaultRowHeight = 12;
            worksheet.DefaultColWidth = 8;
            worksheet.Row(1).Height = 20;
            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#e06666");

            using (ExcelRange rng = worksheet.Cells["A1:B1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Product List";
                rng.AutoFitColumns();
            }
            using (ExcelRange rng = worksheet.Cells["C1:D1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Total(" + list.Count() + ")";
                rng.AutoFitColumns();
            }

            worksheet.Row(2).Height = 20;
            worksheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Row(2).Style.Font.Bold = true;
            worksheet.Row(2).Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[2, 1].Value = "Month";
            worksheet.Cells[2, 2].Value = "Product Id";
            worksheet.Cells[2, 3].Value = "Product Name";
            worksheet.Cells[2, 4].Value = "Category";
            //worksheet.Cells[2, 5].Value = "Description";

            int totalRows = list.Count();
            int i = 0;
            for (int row = 3; row <= totalRows + 2; row++)
            {
                worksheet.Cells[row, 1].Value = list[i].Month;
                worksheet.Cells[row, 2].Value = list[i].GoodId;
                worksheet.Cells[row, 3].Value = list[i].GoodName;
                worksheet.Cells[row, 4].Value = list[i].CategoryName;
                //worksheet.Cells[row, 5].Value = list[i].GoodDescription;
                i++;
            }
            worksheet.Column(1).AutoFit();
            worksheet.Column(2).AutoFit();
            worksheet.Column(3).AutoFit();
            worksheet.Column(4).AutoFit();
            //worksheet.Column(5).AutoFit();


            //Second Category

            worksheet2.TabColor = System.Drawing.Color.Wheat;
            worksheet2.DefaultRowHeight = 12;
            worksheet2.DefaultColWidth = 8;
            worksheet2.Row(1).Height = 20;
            using (ExcelRange rng = worksheet2.Cells["A1:B1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Categories Wise Items";
                rng.AutoFitColumns();
            }
            using (ExcelRange rng = worksheet2.Cells["C1:C1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Total(" + listcategories.Count() + ")";
                rng.AutoFitColumns();
            }


            worksheet2.Row(2).Height = 20;
            worksheet2.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet2.Row(2).Style.Font.Bold = true;
            worksheet2.Row(2).Style.Font.Color.SetColor(Color.Black);
            worksheet2.Cells[2, 1].Value = "Category";
            worksheet2.Cells[2, 2].Value = "Product Id";
            worksheet2.Cells[2, 3].Value = "Product Name";
            //worksheet2.Cells[2, 4].Value = "Description";


            int totalRowsCategory = listcategories.Count();
            int ii = 0;
            for (int row = 3; row <= totalRowsCategory + 2; row++)
            {
                worksheet2.Cells[row, 1].Value = listcategories[ii].CategoryName;
                worksheet2.Cells[row, 2].Value = listcategories[ii].GoodId;
                worksheet2.Cells[row, 3].Value = listcategories[ii].GoodName;
                //worksheet2.Cells[row, 4].Value = listcategories[ii].GoodDescription;
                ii++;
            }
            worksheet2.Column(1).AutoFit();
            worksheet2.Column(2).AutoFit();
            worksheet2.Column(3).AutoFit();
            //worksheet2.Column(4).AutoFit();

            //Third Sharer

            worksheet3.TabColor = System.Drawing.Color.Wheat;
            worksheet3.DefaultRowHeight = 12;
            worksheet3.DefaultColWidth = 8;
            worksheet3.Row(1).Height = 20;
            using (ExcelRange rng = worksheet3.Cells["A1:B1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.Font.Size = 14;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Sharers";
                rng.AutoFitColumns();
            }

            using (ExcelRange rng = worksheet3.Cells["C1:D1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Total(" + listSharers.Count() + ")";
                rng.AutoFitColumns();
            }

            worksheet3.Row(2).Height = 20;
            worksheet3.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet3.Row(2).Style.Font.Bold = true;
            worksheet3.Row(2).Style.Font.Color.SetColor(Color.Black);
            worksheet3.Cells[2, 1].Value = "Month";
            worksheet3.Cells[2, 2].Value = "Sharer Id";
            worksheet3.Cells[2, 3].Value = "Sharer Name";
            worksheet3.Cells[2, 4].Value = "Product Name";



            int totalRowsSharer = listSharers.Count();
            int iii = 0;
            for (int row = 3; row <= totalRowsSharer + 2; row++)
            {
                worksheet3.Cells[row, 1].Value = listSharers[iii].Month;
                //worksheet3.Cells[row, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells[row, 2].Value = listSharers[iii].UserId;
                worksheet3.Cells[row, 3].Value = listSharers[iii].FirstName + " " + listSharers[iii].LastName;
                worksheet3.Cells[row, 4].Value = listSharers[iii].GoodName;

                iii++;
            }
            worksheet3.Column(1).AutoFit();
            worksheet3.Column(2).AutoFit();
            worksheet3.Column(3).AutoFit();
            worksheet3.Column(4).AutoFit();




            //Fourth Borrowers

            worksheet4.TabColor = System.Drawing.Color.Wheat;
            worksheet4.DefaultRowHeight = 12;
            worksheet4.DefaultColWidth = 8;
            worksheet4.Row(1).Height = 20;
            using (ExcelRange rng = worksheet4.Cells["A1:B1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.Font.Size = 14;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Borrowers";
                rng.AutoFitColumns();
            }
            using (ExcelRange rng = worksheet4.Cells["C1:D1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Total(" + listborrowers.Count() + ")";
                rng.AutoFitColumns();
            }
            worksheet4.Row(2).Height = 20;
            worksheet4.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet4.Row(2).Style.Font.Bold = true;
            worksheet4.Row(2).Style.Font.Color.SetColor(Color.Black);
            worksheet4.Cells[2, 1].Value = "Month";
            worksheet4.Cells[2, 2].Value = "Borrower Id";
            worksheet4.Cells[2, 3].Value = "Borrower Name";
            worksheet4.Cells[2, 4].Value = "Product Name";



            int totalRowsBorrowers = listborrowers.Count();
            int k = 0;
            for (int row = 3; row <= totalRowsBorrowers + 2; row++)
            {
                worksheet4.Cells[row, 1].Value = listborrowers[k].Month;
                //worksheet3.Cells[row, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet4.Cells[row, 2].Value = listborrowers[k].UserId;
                worksheet4.Cells[row, 3].Value = listborrowers[k].FirstName + " " + listborrowers[k].LastName;
                worksheet4.Cells[row, 4].Value = listborrowers[k].GoodName;

                k++;
            }
            worksheet4.Column(1).AutoFit();
            worksheet4.Column(2).AutoFit();
            worksheet4.Column(3).AutoFit();
            worksheet4.Column(4).AutoFit();



            string excelName = "Reports: " + DateTime.Now.ToString("dd-MM-yyyy");
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                excelPackage.SaveAs(memoryStream);
                memoryStream.Position = 0;
                TempData[excelName] = memoryStream.ToArray();
            }
            // Note we are returning a filename as well as the handle
            return new JsonResult()
            {
                Data = new { FileGuid = excelName, FileName = excelName + ".xlsx" }
            };
            //return Json("Exported", JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }

        [AllowAnonymous]
        public ActionResult AbusiveReports()
        {
         
            List<ReportAbuseVM> listNew = new List<ReportAbuseVM>();
            List<ReportAbuseModel> list = new List<ReportAbuseModel>();
            listNew = _typeService.GetNewAllAbusiveReports();
            list = _typeService.GetAllAbusiveReports();
            return View(listNew);
        }


        [AllowAnonymous]
        public ActionResult AbusivereportDetail(string itemId)
        {
            ReportAbuseVM obj = new ReportAbuseVM();
            List<ReportAbuseModel> list = new List<ReportAbuseModel>();
         
            try
            {
                obj.reportAbuseDetalList = _typeService.GetNewReportsDetail(Convert.ToInt32(itemId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(obj);
        }

        public string NoIssue(string itemId, string userId)
        {
            string response = "";
            int userID = 0;
            int.TryParse(userId, out userID);
            try
            {
                var res = _typeService.NoIssue(Convert.ToInt32(itemId), Convert.ToInt32(userID));
                if (res)
                {
                    response = "Issue Removed";
                }
                else
                {
                    response = "Something Went Wrong!";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public string SendReminder(string itemId, string userId)
        {
            string response = "";
            try
            {
                var res = _typeService.SendReminder(Convert.ToInt32(itemId), Convert.ToInt32(userId), QuickUrl);
                if (res)
                {
                    response = "Notified";
                }
                else
                {
                    response = "Something Went Wrong!";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public string SetAbusive(string itemId, string userId)
        {
            string response = "";
            try
            {
                var res = _typeService.SetAbusive(Convert.ToInt32(itemId), Convert.ToInt32(userId));
                if (res)
                {
                    response = "Marked As Abusive";
                }
                else
                {
                    response = "Something Went Wrong!";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        [HttpGet]
        public ActionResult AllTransactionReports()
        {
            DetailedReports detailedReports = new DetailedReports();
        
            try
            {
                var ddlCategories = _momentarilyItemDataService.categoriesList();
                var categoryList = new SelectList(ddlCategories.ToList(), "CategoryId", "CategoryName");
                ViewData["DDLCategories"] = categoryList;
                detailedReports.StartRentalDate = DateTime.Now.AddMonths(-1);
                detailedReports.EndRentalDate = DateTime.Now;

                detailedReports.AllTransactionReports = _momentarilyItemDataService.getAllTransactionData();
                detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.RentalStartDate >= detailedReports.StartRentalDate
                 && x.RentalEndDate <= detailedReports.EndRentalDate).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(detailedReports);
        }



        [HttpPost]
        public ActionResult AllTransactionReports(DetailedReports detailedReports)
        {
            try
            {
                var ddlCategories = _momentarilyItemDataService.categoriesList();
                var categoryList = new SelectList(ddlCategories.ToList(), "CategoryId", "CategoryName");
                ViewData["DDLCategories"] = categoryList;
                detailedReports.AllTransactionReports = _momentarilyItemDataService.getAllTransactionData();
                detailedReports.AllTransactionReports = _momentarilyItemDataService.getAllTransactionData().Where(x => x.RentalStartDate >= detailedReports.StartRentalDate
                                     && x.RentalEndDate <= detailedReports.EndRentalDate).ToList();

                if (detailedReports.SharerId != "" && detailedReports.SharerId != null)
                {
                    detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.GoodOwnerName.ToLower().Contains(detailedReports.SharerId.ToLower())).ToList();
                }

                if (detailedReports.BorrowerId != "" && detailedReports.BorrowerId != null)
                {
                    detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.UserName.ToLower().Contains(detailedReports.BorrowerId.ToLower())).ToList();
                }

                if (detailedReports.GoodNameId != "" && detailedReports.GoodNameId != null)
                {
                    detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.GoodName.ToLower().Contains(detailedReports.GoodNameId.ToLower())).ToList();
                }
                if (detailedReports.AmountRangeId > 0)
                {
                    int start;
                    int end;
                    getrange(detailedReports.AmountRangeId, out start, out end);
                    detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.Price >= start && x.Price <= end).ToList();
                }
                if (detailedReports.CategoryId > 0)
                {
                    detailedReports.AllTransactionReports = _momentarilyItemDataService.GetReportByCategory(detailedReports.AllTransactionReports, detailedReports.CategoryId);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            TempData["DivLoading"] = "Loading";
            return View(detailedReports);
        }
        private void getrange(int amountRangeId, out int startRange, out int endrange)
        {
            if (amountRangeId == 1)
            {
                startRange = 0; endrange = 50;
            }

            else if (amountRangeId == 2)
            {
                startRange = 50; endrange = 100;
            }

            else if (amountRangeId == 3)
            {
                startRange = 100; endrange = 200;
            }

            else if (amountRangeId == 4)
            {
                startRange = 200; endrange = 300;
            }
            else if (amountRangeId == 5)
            {
                startRange = 300; endrange = 400;
            }
            else if (amountRangeId == 6)
            {
                startRange = 400; endrange = 500;
            }
            else if (amountRangeId == 7)
            {
                startRange = 500; endrange = 1000;
            }
            else
            {
                startRange = 1000; endrange = 10000000;
            }
        }

        [HttpPost]
        public ActionResult ExportToCsvTransaction(DateTime startRentalDate, DateTime endRentalDate, string searchShareName, string searchBorrowerName, int categoryId, string ItemName, int amountRangeId)        {            DetailedReports detailedReports = new DetailedReports();            detailedReports.AllTransactionReports = _momentarilyItemDataService.getAllTransactionData().Where(x => x.RentalStartDate >= startRentalDate            && x.RentalEndDate <= endRentalDate).ToList();            if (searchShareName != "" && searchShareName != null)            {                detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.GoodOwnerName.ToLower().Contains(searchShareName.ToLower())).ToList();            }            if (searchBorrowerName != "" && searchBorrowerName != null)            {                detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.UserName.ToLower().Contains(searchBorrowerName.ToLower())).ToList();            }            if (ItemName != "" && ItemName != null)            {                detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.GoodName.Contains(ItemName)).ToList();            }            if (amountRangeId > 0)            {                int start;                int end;                getrange(amountRangeId, out start, out end);                detailedReports.AllTransactionReports = detailedReports.AllTransactionReports.Where(x => x.Price >= start && x.Price <= end).ToList();            }            if (categoryId > 0)            {                detailedReports.AllTransactionReports = _momentarilyItemDataService.GetReportByCategory(detailedReports.AllTransactionReports, categoryId);            }            ExcelPackage excelPackage = new ExcelPackage();            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("TransactionReports");            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#666666");            Color colFromText = System.Drawing.ColorTranslator.FromHtml("#d8d6d1");            Color setColorborder = System.Drawing.ColorTranslator.FromHtml("#e9ecef");            using (ExcelRange rng = worksheet.Cells["AE1:ZZ500"])            {                rng.Merge = true;                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;                rng.Style.Border.Left.Color.SetColor(setColorborder);
                //rng.AutoFitColumns();
            }            worksheet.TabColor = System.Drawing.Color.Wheat;            worksheet.DefaultRowHeight = 12;            worksheet.DefaultColWidth = 15;            worksheet.Row(1).Height = 60;            worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            //worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(colFromHex);
            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;            worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;            worksheet.Row(1).Style.Font.Bold = true;            worksheet.Row(1).Style.Font.Color.SetColor(colFromText);            worksheet.Cells[1, 1].Value = "Transaction #";            worksheet.Cells[1, 2].Value = "City";            worksheet.Cells[1, 3].Value = "State";            worksheet.Cells[1, 4].Style.WrapText = true;            worksheet.Cells[1, 4].Value = "Rental Start Date";            worksheet.Cells[1, 5].Style.WrapText = true;            worksheet.Cells[1, 5].Value = "Rental End Date";
            worksheet.Cells[1, 6].Style.WrapText = true;
            worksheet.Cells[1, 6].Value = "Rental Requested On";
            worksheet.Cells[1, 7].Style.WrapText = true;
            worksheet.Cells[1, 7].Value = "Last Modified Date";            worksheet.Cells[1, 8].Value = "Status";            worksheet.Cells[1, 9].Value = "Payment Processed";            worksheet.Cells[1, 10].Value = "Late Fee";            worksheet.Cells[1, 11].Value = "Claim Amount";            worksheet.Cells[1, 12].Style.WrapText = true;            worksheet.Cells[1, 12].Value = "Total Paid Amount";            worksheet.Cells[1, 13].Value = "Sharer Amount";            worksheet.Cells[1, 14].Value = "Refunded Amount";            worksheet.Cells[1, 15].Value = "Pending Amount";            worksheet.Cells[1, 16].Value = "Rental Days";            worksheet.Cells[1, 17].Value = "Rental Cost/Day";
            worksheet.Cells[1, 18].Value = "Total Rental Cost";            worksheet.Cells[1, 19].Value = "Charity Amount";            worksheet.Cells[1, 20].Value = "Service Fee";            worksheet.Cells[1, 21].Value = "Security Deposit";            worksheet.Cells[1, 22].Value = "Discount Amount";            worksheet.Cells[1, 23].Value = "Rental Total";            worksheet.Cells[1, 24].Style.WrapText = true;            worksheet.Cells[1, 24].Value = "Momentarily Earnings";            worksheet.Cells[1, 25].Value = "Owner";            worksheet.Cells[1, 26].Style.WrapText = true;            worksheet.Cells[1, 26].Value = "Owner Received Rating for Transaction";            worksheet.Cells[1, 27].Value = "Borrower";            worksheet.Cells[1, 28].Style.WrapText = true;            worksheet.Cells[1, 28].Value = "Borrower Received Rating for Transaction";            worksheet.Cells[1, 29].Value = "Category";            worksheet.Cells[1, 30].Value = "Description";            using (ExcelRange rng = worksheet.Cells["A1:AD1"])            {                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);                rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;                rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;                rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;                rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;                rng.Style.Border.Top.Color.SetColor(setColorborder);                rng.Style.Border.Bottom.Color.SetColor(setColorborder);                rng.Style.Border.Right.Color.SetColor(setColorborder);                rng.Style.Border.Left.Color.SetColor(setColorborder);            }            for (int j = 0; j < detailedReports.AllTransactionReports.Count(); j++)            {                worksheet.Row(j + 2).Height = 25;                worksheet.Row(j + 2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;                worksheet.Row(j + 2).Style.Font.Color.SetColor(Color.Black);                worksheet.Cells[j + 2, 1].Value = detailedReports.AllTransactionReports[j].TransactionId;                worksheet.Cells[j + 2, 2].Value = detailedReports.AllTransactionReports[j].City;                worksheet.Cells[j + 2, 3].Value = detailedReports.AllTransactionReports[j].State;                worksheet.Cells[j + 2, 4].Value = detailedReports.AllTransactionReports[j].RentalStartDate.ToString("dd/MM/yyyy");                worksheet.Cells[j + 2, 5].Value = detailedReports.AllTransactionReports[j].RentalEndDate.ToString("dd/MM/yyyy");
                worksheet.Cells[j + 2, 6].Value = detailedReports.AllTransactionReports[j].RentalStartDate.ToString("dd/MM/yyyy");
                worksheet.Cells[j + 2, 7].Value = detailedReports.AllTransactionReports[j].ModDate.ToString("dd/MM/yyyy");                worksheet.Cells[j + 2, 8].Value = detailedReports.AllTransactionReports[j].StatusName;                worksheet.Cells[j + 2, 9].Value = detailedReports.AllTransactionReports[j].PaymentProcess;                worksheet.Cells[j + 2, 10].Value = "$" + detailedReports.AllTransactionReports[j].LateFee;                worksheet.Cells[j + 2, 11].Value = "$" + detailedReports.AllTransactionReports[j].ClaimAmount;                worksheet.Cells[j + 2, 12].Value = "$" + detailedReports.AllTransactionReports[j].CustomerCost;                worksheet.Cells[j + 2, 13].Value = "$" + detailedReports.AllTransactionReports[j].OnDisputeAmountForSharer;                worksheet.Cells[j + 2, 14].Value = "$" + detailedReports.AllTransactionReports[j].OnDisputeAmountForBorrower;                worksheet.Cells[j + 2, 15].Value = "$" + detailedReports.AllTransactionReports[j].PendingAmount;                worksheet.Cells[j + 2, 16].Value = detailedReports.AllTransactionReports[j].Days;                worksheet.Cells[j + 2, 17].Value = "$" + detailedReports.AllTransactionReports[j].Price;                worksheet.Cells[j + 2, 18].Value = "$" + detailedReports.AllTransactionReports[j].DaysCost;                worksheet.Cells[j + 2, 19].Value = "$" + detailedReports.AllTransactionReports[j].CustomerCharityCost;                worksheet.Cells[j + 2, 20].Value = "$" + detailedReports.AllTransactionReports[j].CustomerServiceFeeCost;                worksheet.Cells[j + 2, 21].Value = "$" + detailedReports.AllTransactionReports[j].SecurityDeposit;                worksheet.Cells[j + 2, 22].Value = "$" + detailedReports.AllTransactionReports[j].DiscountAmount;                worksheet.Cells[j + 2, 23].Value = "$" + detailedReports.AllTransactionReports[j].CustomerCost;                worksheet.Cells[j + 2, 24].Value = "$" + detailedReports.AllTransactionReports[j].Earnings;                worksheet.Cells[j + 2, 25].Value = detailedReports.AllTransactionReports[j].GoodOwnerName;                worksheet.Cells[j + 2, 26].Value = _momentarilyItemDataService.RatingToOwnerPerRequest(detailedReports.AllTransactionReports[j].TransactionId,                                                    detailedReports.AllTransactionReports[j].GoodOwnerId);                worksheet.Cells[j + 2, 27].Value = detailedReports.AllTransactionReports[j].UserName;                worksheet.Cells[j + 2, 28].Value = _momentarilyItemDataService.RatingToBorrowerPerRequest(detailedReports.AllTransactionReports[j].TransactionId,                                                 detailedReports.AllTransactionReports[j].UserId);                var categories = _momentarilyItemDataService.categoriesListbyGoodId(detailedReports.AllTransactionReports[j].GoodId);                string combindedString = string.Join(",", categories);                worksheet.Cells[j + 2, 29].Value = combindedString;                worksheet.Cells[j + 2, 30].Value = detailedReports.AllTransactionReports[j].Description;            }            worksheet.Column(1).AutoFit();            worksheet.Column(2).AutoFit();            worksheet.Column(3).AutoFit();            worksheet.Column(4).AutoFit();            worksheet.Column(5).AutoFit();            worksheet.Column(6).AutoFit();            worksheet.Column(7).AutoFit();            worksheet.Column(8).AutoFit();            worksheet.Column(9).AutoFit();            worksheet.Column(10).AutoFit();            worksheet.Column(11).AutoFit();            worksheet.Column(12).AutoFit();            worksheet.Column(13).AutoFit();            worksheet.Column(14).AutoFit();            worksheet.Column(15).AutoFit();            worksheet.Column(16).AutoFit();            worksheet.Column(17).AutoFit();            worksheet.Column(18).AutoFit();            worksheet.Column(19).AutoFit();            worksheet.Column(20).AutoFit();            worksheet.Column(21).AutoFit();            worksheet.Column(22).AutoFit();            worksheet.Column(23).AutoFit();            worksheet.Column(24).AutoFit();            worksheet.Column(25).AutoFit();            worksheet.Column(26).AutoFit();            worksheet.Column(27).AutoFit();            worksheet.Column(28).AutoFit();            worksheet.Column(29).AutoFit();            worksheet.Column(30).AutoFit();            worksheet.Column(4).Width = 12;            worksheet.Column(5).Width = 12;
            worksheet.Column(6).Width = 12;
            worksheet.Column(7).Width = 12;            worksheet.Column(12).Width = 13;            worksheet.Column(24).Width = 12;            worksheet.Column(14).Width = 12;            worksheet.Column(26).Width = 12;            worksheet.Column(28).Width = 12;            string excelName = "All Transactions Report: " + DateTime.Now.ToString("dd-MM-yyyy");            using (var memoryStream = new MemoryStream())            {                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");                excelPackage.SaveAs(memoryStream);                memoryStream.Position = 0;                TempData[excelName] = memoryStream.ToArray();            }            return new JsonResult()            {                Data = new { FileGuid = excelName, FileName = excelName + ".xlsx" }            };        }
        [HttpGet]
        public ActionResult GenerateCoupon(int? id)
        {
            UserCouponVM obj = new UserCouponVM();
         
            if (id != null && id > 0)
            {
                obj = _momentarilyItemDataService.getEditCouponDetail(id);
                var ddlCoupons = _momentarilyItemDataService.getCouponList();
                var categoryList = new SelectList(ddlCoupons.ToList(), "Id", "CouponName", obj.CouponType);
                ViewData["DDLCoupons"] = categoryList;
                obj.userCouponVMsList = _momentarilyItemDataService.getAllCouponList();
            }
            else
            {
                obj.ExpiryDate = DateTime.Now;
                obj.CouponDiscountType = 1;
                var ddlCoupons = _momentarilyItemDataService.getCouponList();
                var categoryList = new SelectList(ddlCoupons.ToList(), "Id", "CouponName");
                ViewData["DDLCoupons"] = categoryList;
                obj.userCouponVMsList = _momentarilyItemDataService.getAllCouponList();
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult GenerateCoupon(UserCouponVM userCouponVM)
        {
            var ddlCoupons = _momentarilyItemDataService.getCouponList();
            var categoryList = new SelectList(ddlCoupons.ToList(), "Id", "CouponName", userCouponVM.CouponType);
            ViewData["DDLCoupons"] = categoryList;
            try
            {
                if (userCouponVM.Id != 0)
                {
                    var updateData = _momentarilyItemDataService.UpdateUserCoupon(userCouponVM);
                    TempData["CouponMsg"] = "Modify Successfully.";
                    return RedirectToAction("GenerateCoupon", new { id = 0 });
                }
                else
                {
                    var alreadyExsist = _momentarilyItemDataService.AlreadyExsistByCouponType(userCouponVM.CouponType);
                    if (alreadyExsist == true)
                    {
                        TempData["CouponMsg"] = "Coupon already exsist.";
                    }
                    else
                    {
                        var saveData = _momentarilyItemDataService.SaveUserCoupon(userCouponVM);
                        TempData["CouponMsg"] = "Save Successfully.";
                    }
                    return RedirectToAction("GenerateCoupon", new { id = 0 });
                }
            }
            catch (Exception ex)
            {
                TempData["CouponMsg"] = "Something went wrong!" + ex.Message;
                return View(userCouponVM);
            }

        }

        public bool DeleteCoupon(int Id)
        {
            var result = false;
            try
            {
                result = _momentarilyItemDataService.DeleteCoupon(Id);
                TempData["DeleteMsg"] = "Coupon deleted successfully.";
                return result;
            }
            catch (Exception ex)
            {
                TempData["DeleteMsg"] = "Something went wrong!" + ex.Message;
                return false;
            }
        }

        [AllowAnonymous]
        public ActionResult BlockedCoupon(int CouponId, bool checkedValue)
        {
            int result = _momentarilyItemDataService.blockedCoupon(CouponId, checkedValue);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutomaticallyGenerateCouponCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 9)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            //string genrateCouponCode = new Random().Next(100000000, 999999999).ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AllDisputes()
        {
          
            var result = _accountDataService.GetAllDisputes();

            return View(result);
        }
        public ActionResult Detail(int id)
        {
            var result = _accountDataService.GetDispute(id);
            return PartialView("_DisputeDetail", result);
        }
        [HttpGet]
        public ActionResult Resolve(int id)
        {
            
            CreateReasonViewBag();
            var result = _accountDataService.GetResolvedDisputeInfo(id);
            if (result != null)
            {
                return View(result);
            }
            return RedirectToHome();
        }

        [HttpPost]        public ActionResult Resolve(ResolvedDisputeViewModel result)        {            try            {                CreateReasonViewBag();                var request = _accountDataService.GetRequest(result.RequestId);                if (request.CreateResult == CreateResult.Success && request != null && request.Obj != null)                {                    var payout = new Payout();                    payout.sender_batch_header = new PayoutSenderBatchHeader();                    payout.sender_batch_header.sender_batch_id = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8);                    payout.sender_batch_header.email_subject = "Payment Processed. Request Id= " + result.RequestId;                    payout.items = new List<PayoutItem>();                    APIContext apiContext = PaypalConfiguration.GetAPIContext();                    if (result.BorrowerShare > 0)                    {                        Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Create resolved dispute payment process for borrower with request id" + result.RequestId));                        var BorrowerPaypalDetail = _accountDataService.getPaypalDetail(request.Obj.UserId);                        if (BorrowerPaypalDetail != null && BorrowerPaypalDetail.UserId > 0)                        {                            var borroweritem = new PayoutItem();                            var borroweramount = new Currency();                            borroweramount.value = Convert.ToString(Math.Round(result.BorrowerShare, 2));                            borroweramount.currency = "USD";                            if (BorrowerPaypalDetail.PaymentType == (int)GlobalCode.Email)                            {                                borroweritem.recipient_type = PayoutRecipientType.EMAIL;                                borroweritem.receiver = BorrowerPaypalDetail.PaypalBusinessEmail;                            }                            else                            {                                borroweritem.recipient_type = PayoutRecipientType.PHONE;                                borroweritem.receiver = BorrowerPaypalDetail.PhoneNumber;                            }                            borroweritem.amount = borroweramount;                            borroweritem.note = "Dispute Resolved. Request Id " + result.RequestId;                            borroweritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Request_Id" + result.RequestId;                            payout.items.Add(borroweritem);                        }                    }                    if (result.OwnerShare > 0)                    {                        Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Create resolved dispute payment process for sharer with request id" + result.RequestId));                        var sharerId = _accountDataService.getUserIdByGoodId(request.Obj.GoodId);                        var SharerPaypalDetail = _accountDataService.getPaypalDetail(sharerId);                        if (SharerPaypalDetail != null && SharerPaypalDetail.UserId > 0)                        {                            var shareritem = new PayoutItem();                            var shareramount = new Currency();                            shareramount.value = Convert.ToString(Math.Round(result.OwnerShare, 2));                            shareramount.currency = "USD";                            if (SharerPaypalDetail.PaymentType == (int)GlobalCode.Email)                            {                                shareritem.recipient_type = PayoutRecipientType.EMAIL;                                shareritem.receiver = SharerPaypalDetail.PaypalBusinessEmail;                            }                            else                            {                                shareritem.recipient_type = PayoutRecipientType.PHONE;                                shareritem.receiver = SharerPaypalDetail.PhoneNumber;                            }                            shareritem.amount = shareramount;                            shareritem.note = "Dispute Resolved. Request Id " + result.RequestId;                            shareritem.sender_item_id = System.Guid.NewGuid().ToString().Substring(0, 8) + "Request_Id" + result.RequestId;                            payout.items.Add(shareritem);                        }                    }                    if (payout.items.Count > 0)                    {                        var createdPayout = payout.Create(apiContext, false);                        var payoutDetail = Payout.Get(apiContext, createdPayout.batch_header.payout_batch_id);

                        var ResolvedDispute = _accountDataService.SaveResolvedDisputeDetail(result);                        if (ResolvedDispute == true)                        {                            Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Save resolved dispute payment process detail with request id" + result.RequestId + "and user id" + request.Obj.UserId));                        }                    }                    request.Obj.StatusId = (int)UserRequestStatus.ClosedWithDispute;                    _accountDataService.UpdateGoodRequest(request.Obj);                    string userEmail = _momentarilyItemDataService.getUserEmail(request.Obj.UserId);                    bool checkExsistSubscriber = _helperAccount.ExsistSubscriber(userEmail);                    if (checkExsistSubscriber == false)                    {                        string subscribertURL = "/Account/NewsLetterSubscribe?Email=" + userEmail;                        var sendMsgSubscriber = _sendMessageService.SendEmailNewsLetterTemplate(userEmail, subscribertURL);                    }                    _sendMessageService.SendEmailAfterTransationComplete(userEmail, request.Obj.User.FullName, request.Obj.Id);                    TempData["resolved"] = "resolved";                    return RedirectToAction("AllDisputes");                }            }            catch (Exception ex)            {                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Resolved dispute payment process fail. Ex: {0}.", ex));                throw ex;            }            return View(result);        }
        public void CreateReasonViewBag()
        {
            List<SelectListItem> reasons = new List<SelectListItem>();
            reasons.Add(new SelectListItem { Text = "Late", Value = "1" });
            reasons.Add(new SelectListItem { Text = "Not Received", Value = "2" });
            reasons.Add(new SelectListItem { Text = "Damaged", Value = "3" });
            reasons.Add(new SelectListItem { Text = "Late And Damaged", Value = "4" });
            reasons.Add(new SelectListItem { Text = "Stolen", Value = "5" });
            reasons.Add(new SelectListItem { Text = "Lost", Value = "6" });
            reasons.Add(new SelectListItem { Text = "Delivered Late", Value = "7" });
            ViewBag.Reasons = reasons;
        }


        [HttpGet]        public ActionResult AdminMessages()        {
                       if (!_helper.UserHasAccess())                return RedirectToHome();            var result = _Messagehelper.GetMessages();            if (result == null) return RedirectToHome();

            var shape = _shapeFactory.BuildShape(null, result, PageName.UserMessage.ToString());

            return shape != null ? DisplayShape(shape) : RedirectToHome();        }

        [HttpGet]        public ActionResult AdminConversation(string userId)        {
                        try            {                if (!_helper.UserHasAccess())                    return RedirectToHome();                int check = 0;                Int32.TryParse(userId, out check);                if (check != 0)                {                    var result = _Messagehelper.GetConversation(check);                    if (result == null) return RedirectToHome();                    var shape = _shapeFactory.BuildShape(null, result, PageName.UserMessageConversation.ToString());                    return shape != null ? DisplayShape(shape) : RedirectToHome();                }                else                {                    byte[] data = Convert.FromBase64String(userId);                    string decodedUserIdString = Encoding.UTF8.GetString(data);                    int convertedUserId = Convert.ToInt32(decodedUserIdString);                    var result = _Messagehelper.GetConversation(convertedUserId);                    if (result == null) return RedirectToHome();                    var shape = _shapeFactory.BuildShape(null, result, PageName.UserMessageConversation.ToString());                    return shape != null ? DisplayShape(shape) : RedirectToHome();                }            }            catch            {                return RedirectToHome();            }        }

        [HttpPost]
        public bool AdminConversation(UserMessageCreateModel messageModel)
        {
            if (messageModel == null || messageModel.ReceiverId == 0 || messageModel.ReceiverId == UserId.Value
                || string.IsNullOrWhiteSpace(messageModel.Message))
                return false;
            var req = HttpContext.Request;

            messageModel.AuthorId = UserId.Value;
            messageModel.IsSystem = false;
            var result = _userMessageService.AddMessage(messageModel);
            if (result.CreateResult == CreateResult.Success && messageModel.files != null && messageModel.files.Count > 0)
            {
                var imagesPath = new List<string>();
                foreach (var image in messageModel.files)
                {
                    var imgArr = image.Split(',');

                    if (imgArr.Length > 1)
                    {
                        string part = image.Substring(0, image.IndexOf(',')).Substring(0, image.IndexOf(';'));
                        string type = part.Substring(part.LastIndexOf("/") + 1);
                        var filePath = SaveImageFromBase64(imgArr[1], type);
                        imagesPath.Add(filePath);
                    }
                }
                if (imagesPath != null && imagesPath.Count > 0)
                {
                    _userMessageService.SaveMessageImagesPath(imagesPath);
                }
            }


            return true;
        }

        [HttpGet]        public JsonResult GetUnreadMessages(string userId)        {            List<UserMessageViewModel> messagelist = new List<UserMessageViewModel>();
            var result = _Messagehelper.GetConversation(Convert.ToInt32(userId));
            if (result != null && result.Messages != null && result.Messages.Count > 0)
            {
                messagelist = result.Messages.Where(x => x.IsRead == false).OrderBy(x => x.DateCreated).ToList();
                //messagelist.Remove("");
                _Messagehelper.SetIsRead(Convert.ToInt32(userId));                foreach (var message in messagelist)
                {                    var msg = message.Message;                    message.Message = msg.Replace(@"\", string.Empty);                }
            }            return Json(new { messagelist = messagelist }, JsonRequestBehavior.AllowGet);        }

        [HttpGet]        public int GetUnreadCount(string userId)        {            int count = 0;            try            {
                var result = _Messagehelper.GetConversation(Convert.ToInt32(userId));
                if (result != null && result.Messages.Count > 0)                {                    count = result.Messages.Where(x => x.IsRead == false).Count();                }            }            catch            {                count = 0;            }            return count;        }

        public static string Decrypt(string cipher)        {            using (var md5 = new MD5CryptoServiceProvider())            {                using (var tdes = new TripleDESCryptoServiceProvider())                {                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));                    tdes.Mode = CipherMode.ECB;                    tdes.Padding = PaddingMode.PKCS7;                    using (var transform = tdes.CreateDecryptor())                    {                        byte[] cipherBytes = Convert.FromBase64String(cipher);                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);                        return UTF8Encoding.UTF8.GetString(bytes);                    }                }            }        }

        public static string Encrypt(string text)        {            using (var md5 = new MD5CryptoServiceProvider())            {                using (var tdes = new TripleDESCryptoServiceProvider())                {                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));                    tdes.Mode = CipherMode.ECB;                    tdes.Padding = PaddingMode.PKCS7;                    using (var transform = tdes.CreateEncryptor())                    {                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);                        return Convert.ToBase64String(bytes, 0, bytes.Length);                    }                }            }        }

        [HttpGet]
        public ActionResult ResolvedDisputes()
        {
            
            var result = _accountDataService.GetResolvedDisputes();
            return View(result);
        }

        private string SaveImageFromBase64(string img, string type)
        {
            string basePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/MessageImages");
            byte[] imageBytes = Convert.FromBase64String(img);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            string newFile = "";

            if (type.ToUpper() == "PNG")
            {
                newFile = Guid.NewGuid().ToString() + ".png";
            }
            else if (type.ToUpper() == "JPG" || type.ToUpper() == "JPEG")
            {
                newFile = Guid.NewGuid().ToString() + ".jpg";
            }
            else if (type.ToUpper() == "PLAIN")
            {
                newFile = Guid.NewGuid().ToString() + ".txt";
            }
            else
            {
                newFile = Guid.NewGuid().ToString() + "." + type;
            }

            var path = Path.Combine(basePath, newFile);
            bool exists = System.IO.Directory.Exists(basePath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(basePath);
            }
            if (imageBytes.Length > 0)
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    stream.Write(imageBytes, 0, imageBytes.Length);
                    stream.Flush();
                }
            }

            path = path.Replace(basePath, "").Replace("\\", "/");
            return path;
        }



        public ActionResult ClearLogs(int id = 0)
        {
           
            if (id > 0)
            {
                var userLogs = _userLogService.DeleteUserLogs(id);
            }
            return RedirectToAction("UserLogs", new { id = id });
        }
        public ActionResult UserLogs(int id = 0)
        {
            List<LogEntryViewModel> list = new List<LogEntryViewModel>();
         
            if (id > 0)
            {
                var userLogs = _userLogService.GetUserLogs(id);
                if (userLogs != null && userLogs.Count > 0)
                {
                    return View(userLogs);
                }
                else
                {
                    return View(list);
                }
            }
            return View(list);
        }

        public ActionResult ExportLogs(int userId)
        {
            List<LogEntryViewModel> list = new List<LogEntryViewModel>();

            list = _userLogService.GetUserLogs(userId);

            ExcelPackage excelPackage = new ExcelPackage();
            //Create the WorkSheet
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("UserLogs");

            worksheet.TabColor = System.Drawing.Color.Wheat;
            worksheet.DefaultRowHeight = 12;
            worksheet.DefaultColWidth = 8;
            worksheet.Row(1).Height = 20;
            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#e06666");

            using (ExcelRange rng = worksheet.Cells["A1:D1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Use Log List";
                rng.AutoFitColumns();
            }
            using (ExcelRange rng = worksheet.Cells["E1:G1"])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Font.Size = 14;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(colFromHex);
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Value = "Total(" + list.Count() + ")";
                rng.AutoFitColumns();
            }



            worksheet.Row(2).Height = 25;
            worksheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Row(2).Style.Font.Bold = true;
            worksheet.Row(2).Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[2, 1].Value = "Log Id";
            worksheet.Cells[2, 2].Value = "Create Date";
            worksheet.Cells[2, 3].Value = "Source Name";
            worksheet.Cells[2, 4].Value = "Severity";
            worksheet.Cells[2, 5].Value = "Session Id";
            worksheet.Cells[2, 6].Value = "IP Address";
            worksheet.Cells[2, 7].Value = "Message";


            int totalRows = list.Count();
            int i = 0;
            for (int row = 3; row <= totalRows + 2; row++)
            {
                worksheet.Cells[row, 1].Value = list[i].Id;
                worksheet.Cells[row, 2].Value = list[i].CreateDate;
                worksheet.Cells[row, 3].Value = list[i].SourceName;
                worksheet.Cells[row, 4].Value = list[i].Severity;
                worksheet.Cells[row, 5].Value = list[i].SessionId;
                worksheet.Cells[row, 6].Value = list[i].IpAddress;
                worksheet.Cells[row, 7].Value = list[i].Message;
                i++;
                worksheet.Row(row).Height = 20;
            }
            worksheet.Column(1).AutoFit();
            worksheet.Column(2).AutoFit();
            worksheet.Column(3).AutoFit();
            worksheet.Column(4).AutoFit();
            worksheet.Column(5).AutoFit();
            worksheet.Column(6).AutoFit();
            worksheet.Column(7).AutoFit();

            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 30;
            worksheet.Column(4).Width = 20;
            worksheet.Column(5).Width = 40;
            worksheet.Column(6).Width = 30;
            worksheet.Column(7).Width = 120;

            worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            string excelName = "User Logs: " + DateTime.Now.ToString("dd-MM-yyyy");
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                excelPackage.SaveAs(memoryStream);
                memoryStream.Position = 0;
                TempData[excelName] = memoryStream.ToArray();
            }

            return new JsonResult()
            {
                Data = new { FileGuid = excelName, FileName = excelName + ".xlsx" }
            };
        }

        public ActionResult AllCovidOrders()
        {
            
            var AllCovidOrders = _healthDataService.GetAllCovidGoodsOrder();
            return View(AllCovidOrders);
        }

        public ActionResult OrderDetail(int id)
        {
          
            var result = _healthDataService.GetCovidGoodsOrder(id);
            return PartialView("_OrderDetail", result);
        }

        public bool UpdateOrder(int id)
        {
            var detail = _healthDataService.GetCovidGoodsOrder(id);
            var result = _healthDataService.UpdateCovidGoodOrder(id);
            if (result)
            _sendMessageService.SendForUserMessageAfterCovidRentalClose(detail.BuyerEmailId,detail.FullName);
            return result;
        }

        public ActionResult StockDetail()
        {
            StockDetailViewModel model = new StockDetailViewModel();
            var covidgoods = _healthDataService.GetAllCovidGoods();
            ViewBag.covidgoods = covidgoods.ToList();
            var stockdetail = _Stockhelper.GetAllStockDetail();
            model.stockDetails = stockdetail;
            model.stockmasterAllDetails = _Stockhelper.GetAllStockMasterDetail();
            return View(model);
        }

        [HttpPost]
        public ActionResult StockDetail(StockDetailViewModel model)
        {
            try
            {
                var covidgoods = _healthDataService.GetAllCovidGoods();
                ViewBag.covidgoods = covidgoods.ToList();
                var stockdetail = _Stockhelper.SaveStockDetail(model);
                if (stockdetail != null && stockdetail.Id > 0)
                {
                    return RedirectToAction("StockDetail");
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("StockDetail");
        }

        public ActionResult StockMaster()
        {
            List<StockMasterDetailVM> model = new List<StockMasterDetailVM>();
            model = _Stockhelper.GetAllStockMasterDetail();
            return View(model);
        }
        public bool DeleteStockDetail(int StockId)
        {
            var result = false;
            try
            {
                result = _Stockhelper.DeleteStockDetail(StockId);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetAvailableCovidItemCount(int Id)
        {
            int count = 0;
           StockMasterDetailVM model = new StockMasterDetailVM();
            model = _Stockhelper.GetStockMasterDetail(Id);
            count = model.QuantityLeft;
            return count;
        }
        public ActionResult UpdateStockDetail(int StockId,int? QuantityLeft)        {            var covidgoods = _healthDataService.GetAllCovidGoods();            ViewBag.covidgoods = covidgoods.ToList();            var result = _Stockhelper.GetStockDetail(StockId, QuantityLeft);            TempData["iserror"] = "false";            return View("UpdateStockDetail", result);        }        [HttpPost]        public ActionResult UpdateStockDetail(StockVM model)        {            var covidgoods = _healthDataService.GetAllCovidGoods();            ViewBag.covidgoods = covidgoods.ToList();            var result = _Stockhelper.UpdateStockDetail(model);            if (result == true)            {                TempData["isupdated"] = "true";                return RedirectToAction("StockDetail");            }            TempData["iserror"] = "true";            return View(result);        }

        public ActionResult UserReviews(int userId)
        {
            BookingRankViewModal bookingRankViewModal = new BookingRankViewModal();
            bookingRankViewModal = _userService.GetUserReviews(userId);
            return View(bookingRankViewModal);
        }

        public ActionResult UpdateGoodBookingRank(int id, int UserId)        {            BookingRankViewModal bookingRankViewModal = new BookingRankViewModal();
            _userService.GetupdateGoodBookingRank(id);
            return RedirectToAction("UserReviews",new { userId = UserId });        }
    }
}