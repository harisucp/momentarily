using System.Web.Mvc;
using System.Linq;
using Apeek.Common;
using Apeek.Core.Services;
using Apeek.Web.Framework.Controllers;
using Momentarily.UI.Service.Services;
using Momentarily.ViewModels.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Apeek.Web.Framework.ControllerHelpers;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class SearchController : FrontendController
    {
        private readonly ICategoryService _categoryService;
        private readonly IMomentarilyItemTypeService _typeService;
        private readonly IMomentarilyItemDataService _itemDataService;
        private readonly ISettingsDataService _settingsDataService;
        private readonly UserControllerHelper _helper;
        public SearchController(ICategoryService categoryService, IMomentarilyItemTypeService typeService, IMomentarilyItemDataService itemDataService, ISettingsDataService settingsDataService)
        {
            _categoryService = categoryService;
            _typeService = typeService;
            _itemDataService = itemDataService;
            _settingsDataService = settingsDataService;
            _helper = new UserControllerHelper();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Map", new MomentarilyItemSearchModel());
        }
        [HttpGet]
        public ActionResult Map(MomentarilyItemSearchModel searchModel)
        {
            var userID = UserId.HasValue;
            var checkWithoutLogin = _settingsDataService.GetBetaVersion();
            if (checkWithoutLogin == "Beta" && !userID)
            {
                TempData["checkVersionWithoutLogin"] = "VersionWithoutLogin";
                return RedirectToAction("Index", "Home");
            }


            ViewBag.Title = "Borrow - Browse Thousands of Items Available | momentarily";
            ViewBag.Description = "Borrow and save on momentarily by browsing from thousands of items by people willing to share items for a small fraction of the actual cost.";
            var viewSearchModel = new MomentarilyItemSearchViewModel
            {
                Categories = _categoryService.GetAllCategories()
                .Where(x => x.IsRoot == false && x.ParentId == 1)
                .Select(x => new MomentarilyCategoryModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImagePath = _settingsDataService.GetImgFileServerUrl() + @"/Category/" + x.ImageFileName
                })
                .OrderBy(x => x.Name).ToList(),
                Types = _typeService.GetAllTypes(),
                SearchModel = new MomentarilyItemSearchModel(),
                Result = new FilteredGoodsModel()
            };
            Session["current_user"] =Convert.ToInt32(UserId);
            if (searchModel.Location != null && searchModel.Location != "")
            {
                viewSearchModel.SearchModel.Location = searchModel.Location;
                viewSearchModel.SearchModel.Latitude = searchModel.Latitude;
                viewSearchModel.SearchModel.Longitude = searchModel.Longitude;
            }
            else
            {
                viewSearchModel.SearchModel.Location = "";
                viewSearchModel.SearchModel.Latitude = 0;
                viewSearchModel.SearchModel.Longitude =0;
            }
            if (searchModel.CategoryId.HasValue && searchModel.CategoryId!=12)
                viewSearchModel.SearchModel.CategoryId = searchModel.CategoryId;
            if (searchModel.DateStart.HasValue)
                viewSearchModel.SearchModel.DateStart = searchModel.DateStart;
            if (searchModel.DateEnd.HasValue)
                viewSearchModel.SearchModel.DateEnd = searchModel.DateEnd;
            if (searchModel.PriceFrom.HasValue)
                viewSearchModel.SearchModel.PriceFrom = searchModel.PriceFrom;
            if (searchModel.PriceTo.HasValue)
                viewSearchModel.SearchModel.PriceTo = searchModel.PriceTo;
            viewSearchModel.SearchModel.Page = searchModel.Page;
            viewSearchModel.SearchModel.PageSize = searchModel.PageSize;
            viewSearchModel.SearchModel.Radius = searchModel.Radius;
            viewSearchModel.SearchModel.Keyword = searchModel.Keyword;
            viewSearchModel.SearchModel.RentPeriod = searchModel.RentPeriod;
           //var filteredItems = _itemDataService.GetFilteredItems(viewSearchModel.SearchModel).Obj;
            //viewSearchModel.Result = new FilteredGoodsModel
            //{
            //    Count = filteredItems.Count,
            //    Goods = filteredItems.Goods
            //};
           
            viewSearchModel.Result = new FilteredGoodsModel
            {
                Count = 0,
                Goods = new List<MomentarilyItemMapViewModel>()
            };
            var shape = _shapeFactory.BuildShape(null, viewSearchModel, PageName.Search.ToString());
            return DisplayShape(shape);
        }
        [HttpPost]
        public ActionResult OnEnterPress(string searchModel1, string Filter)
        {
            MomentarilyItemSearchModel SearchModel = JsonConvert.DeserializeObject<MomentarilyItemSearchModel>(searchModel1);
            SearchModel.Keyword = Filter;
            return RedirectToAction("Map", SearchModel);
        }
        [HttpPost]
        public JsonResult SearchMap(MomentarilyItemSearchViewModel searchModel1, string Filter)
       {
            //MomentarilyItemSearchModel searchModel = JsonConvert.DeserializeObject<MomentarilyItemSearchModel>(searchModel1);
            MomentarilyItemSearchModel searchModel = searchModel1.SearchModel;
            var viewSearchModel = new MomentarilyItemSearchViewModel
            {
                Categories = _categoryService.GetAllCategories()
                .Where(x => x.IsRoot == false && x.ParentId == 1)
                .Select(x => new MomentarilyCategoryModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImagePath = _settingsDataService.GetImgFileServerUrl() + @"/Category/" + x.ImageFileName
                })
                .OrderBy(x => x.Name).ToList(),
                Types = _typeService.GetAllTypes(),
                SearchModel = new MomentarilyItemSearchModel(),
                Result = new FilteredGoodsModel()
            };
            if (searchModel.CategoryId.HasValue)
                viewSearchModel.SearchModel.CategoryId = searchModel.CategoryId;
            if (searchModel.DateStart.HasValue)
                viewSearchModel.SearchModel.DateStart = searchModel.DateStart;
            if (searchModel.DateEnd.HasValue)
                viewSearchModel.SearchModel.DateEnd = searchModel.DateEnd;
            if (searchModel.PriceFrom.HasValue)
                viewSearchModel.SearchModel.PriceFrom = searchModel.PriceFrom;
            if (searchModel.PriceTo.HasValue)
                viewSearchModel.SearchModel.PriceTo = searchModel.PriceTo;
            viewSearchModel.SearchModel.Page = searchModel.Page;
            viewSearchModel.SearchModel.PageSize = searchModel.PageSize;
            viewSearchModel.SearchModel.Radius = searchModel.Radius;
            //viewSearchModel.SearchModel.Keyword = searchModel.Keyword;
            viewSearchModel.SearchModel.Keyword = Filter;
            viewSearchModel.SearchModel.RentPeriod = 0;
            var filteredItems = _itemDataService.GetFilteredItems(viewSearchModel.SearchModel).Obj;
            viewSearchModel.Result = new FilteredGoodsModel
            {
                Count = filteredItems.Count,
                Goods = filteredItems.Goods
            };
              return Json(viewSearchModel.Result.Goods, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Item(int id)
        {
            var user = _helper.GetUser();
            if (user != null)
            {
                if (user.Verified && user.IsMobileVerified)
                {
                    var result = _itemDataService.GetItem(id);
                    if (result.CreateResult == CreateResult.Success)
                    {
                        result.Obj.CurrentUserIsOwner = this.UserId != null && this.UserId.Value == result.Obj.User.Id;
                        var shape = _shapeFactory.BuildShape(null, result.Obj, PageName.Search.ToString());
                        return DisplayShape(shape);
                    }
                }
            }
            return RedirectToAction("UserEmailConfirm", "User");
        }
        [HttpPost]
        public ActionResult GetDates(int id)
        {
            var result = _itemDataService.GetItem(id);
            if (result.CreateResult == CreateResult.Success)
            {
                result.Obj.CurrentUserIsOwner = this.UserId != null && this.UserId.Value == result.Obj.User.Id;
                var shape = _shapeFactory.BuildShape(null, result.Obj, PageName.Search.ToString());
                return PartialView("_AvailableDates", DisplayShape(shape));
            }
            return RedirectToHome();
        }

        [HttpPost]
        public string ReportAbuse(string goodId)
        {
            string response = "";
            try
            {
                if(UserId==null || UserId.Value<=0)
                {
                    response = "Please login to your account to add report for this item";
                }
                else
                {
                    var result = _typeService.AddReportAbuse(Convert.ToInt32(goodId),UserId.Value);
                    if (result != null)
                    {
                        response = "Report Sent to Admin.";
                    }
                    else
                    {
                        response = "Something went wrong!";
                    }
                }
            }
            catch(Exception ex)
            {
                response = "Error!";
            }
            return response;
        }
        public bool CheckVersion()        {            bool result = false;

            var checkSiteVersion = _settingsDataService.GetBetaVersion();
            if (checkSiteVersion == "Beta")
            {
                result = true;
            }

            return result;        }
    }
}