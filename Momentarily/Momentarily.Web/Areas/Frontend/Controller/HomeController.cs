using System.Web.Mvc;
using Apeek.Common;
using System;
using System.Linq;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.Controllers;
using Momentarily.ViewModels.Models;
using Newtonsoft.Json;
using Momentarily.UI.Service.Services;
using Apeek.Web.Framework.ControllerHelpers;
using System.Collections.Generic;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Controllers;
using System.Web;
using CaptchaMvc.HtmlHelpers;
using System.Web.UI;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class HomeController : FrontendController
    {
        public static string SessionId = "";
        public static string IpAddress = "";
        public static int? CurrentUserId = null;
        private readonly ICategoryService _categoryService;
        private readonly ISendMessageService _sendMessageService;
        private readonly ISettingsDataService _settingsDataService;
        private readonly IMomentarilyItemTypeService _typeService;
        private readonly IMomentarilyItemDataService _itemDataService;
        private readonly AccountControllerHelper<MomentarilyRegisterModel> _helper;
        private readonly ISettingsDataService _settings;
        public HomeController(ICategoryService categoryService, ISendMessageService sendMessageService, ISettingsDataService settingsDataService, IMomentarilyItemTypeService typeService, IMomentarilyItemDataService itemDataService)
        {
            _categoryService = categoryService;
            _sendMessageService = sendMessageService;
            _settingsDataService = settingsDataService;
            _typeService = typeService;
            _itemDataService = itemDataService;
            _helper = new AccountControllerHelper<MomentarilyRegisterModel>();
            _settings = new SettingsDataService();
        }
        public ActionResult Index()        {            GetUpdateContextDetail();            if (Convert.ToBoolean(Session["IsAdmin"]) == true)            {                _helper.LogOff();                Session["IsAdmin"] = false;                Session["IsAutherised"] = false;                return RedirectToAction("Index");            }            ViewBag.Title = "Share & Borrow Items - momentarily";            ViewBag.Description = "Largest community to share or borrow almost any item such as clothing, bikes, office space, and lots more. momentarily is the p2p rental marketplace.";            ViewBag.GoogleSiteVerification = "tG02v-ZqX3p_2nyAe1D9sUG1RSJuHLJ51FGpjBAIuZM";            var model = new MomentarilyItemSearchViewModel()            {                SearchModel = new MomentarilyItemSearchModel(),                Categories = _categoryService.GetAllCategories()                .Where(x => x.IsRoot == false && x.ParentId == 1)                .Select(x => new MomentarilyCategoryModel
                {                    Id = x.Id,                    Name = x.Name,                    ImagePath = _settingsDataService.GetImgFileServerUrl() + @"/Category/" + x.ImageFileName
                })
                .OrderBy(x => x.Name).ToList()            };

            Session["IsAdmin"] = false;


            ViewBag.RecentlyRentedItems = _itemDataService.GetMostRecentlyRentedProduct();            ViewBag.FeaturedProduct = _itemDataService.GetMostFeaturedProduct();            TempData["CheckSiteVersion"] = _settings.GetBetaVersion();
            var userVerifyId = UserId.HasValue;
            TempData["CheckForCBVerifyAccount"] = userVerifyId;
            model.Categories = model.Categories.Where(x => x.Name != "All Categories" && x.Name != "Other").ToList();            var shape = _shapeFactory.BuildShape(null, model, PageName.Home.ToString());            return DisplayShape(shape);        }

        public void GetUpdateContextDetail()
        {
            try
            {
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                    {
                        SessionId = HttpContextFactory.Current.Session.SessionID;
                        if (HttpContextFactory.Current.Request != null)
                           IpAddress = HttpContextFactory.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    if (ContextService.AuthenticatedUser != null)
                        CurrentUserId = ContextService.AuthenticatedUser.UserId;
                }
            }
            catch(Exception ex)
            {

            }
        }
        public ActionResult LinkExpire()
        {
            ViewBag.Title = "momentarily";
            return View();
        }
        public ActionResult Terms()
        {
            var shape = _shapeFactory.BuildShape(null, new object(), PageName.Home.ToString());
            return DisplayShape(shape);
        }
        public ActionResult PrivacyPolicy()
        {
            var shape = _shapeFactory.BuildShape(null, new object(), PageName.Home.ToString());
            return DisplayShape(shape);
        }
        [HttpGet]
        public ActionResult FAQ()
        {
            ViewBag.Title = "Frequently Asked Questions | momentarily";
            ViewBag.Description = "Get all commonly asked questions about our platform momentarily answered";

            FAQVM model = new FAQVM();
            ViewBag.FAQCheck = false;
            model.FAQLists = _itemDataService.GetFAQs();
            return View(model);
        }
        [HttpPost]
        public ActionResult FAQ(FAQVM fAQVM)
        {
            ViewBag.FAQCheck = true;
            fAQVM.FAQLists = _itemDataService.GetFAQs();
            List<FAQList> list = new List<FAQList>();
            if (fAQVM.Filter!=null && fAQVM.Filter!="")
            {
                foreach(var item in fAQVM.FAQLists)
                {
                    if(item.Question.ToLower().Contains(fAQVM.Filter.ToLower()) || item.Answer.ToLower().Contains(fAQVM.Filter.ToLower()))
                    {
                        list.Add(item);
                    }
                   
                }
                fAQVM.FAQLists = list;
            }
            return View(fAQVM);
        }

        //[HttpGet]
        //public ActionResult ContactUs()
        //{
        //    ViewBag.Title = "Help Centre - Contact Us | momentarily";
        //    ViewBag.Description = "Contact us for issues regarding using the momentarily platform.";
        //    var model = new ContactUsEntry();
    
        //    var shape = _shapeFactory.BuildShape(null, model, PageName.Home.ToString());
        //    return DisplayShape(shape);
        //}
        //[HttpPost]
        //public ActionResult ContactUs(Shape<ContactUsEntry> shape)
        //{
        //    if (!this.IsCaptchaValid("Validate your captcha"))
        //    {
        //        ViewBag.ErrorMessage = "Invalid Captcha";
        //        return DisplayShape(shape);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var result = _sendMessageService.SendEmailContactUs(shape.ViewModel);

        //        shape.IsError = !result;
        //        if (result)
        //        {
        //            shape.ViewModel = new ContactUsEntry();
        //            ViewBag.IsEmailSent = "True";
        //        }
        //        else
        //        {
        //            ViewBag.IsEmailSent = "False";
        //        }
        //    }
        //    return DisplayShape(shape);
        //}
        public ActionResult HowItWorks()
        {
            ViewBag.Title = "How momentarily Works | Share & Earn; Borrow & Save | momentarily";
            ViewBag.Description = "Find out how momentarily works, how we help you make money from items you own and save money on items you need for a short period";
            var shape = _shapeFactory.BuildShape(null, new object(), PageName.Home.ToString());
            return DisplayShape(shape);
        }
        [HttpPost]
        public JsonResult SearchMap(string searchModel1, string Filter)
        {
            MomentarilyItemSearchModel searchModel = JsonConvert.DeserializeObject<MomentarilyItemSearchModel>(searchModel1);
            //MomentarilyItemSearchModel searchModel = new MomentarilyItemSearchModel();
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
            if (searchModel.Location != null)
            {
                viewSearchModel.SearchModel.Location = searchModel.Location;
                viewSearchModel.SearchModel.Latitude = searchModel.Latitude;
                viewSearchModel.SearchModel.Longitude = searchModel.Longitude;
            }
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


        public ActionResult AboutUs()
        {

            ViewBag.Title = "About the momentarily Team";
            ViewBag.Description = "momentarily is a local Peer-to-Peer Rental Marketplace to rent almost anything from bikes to work tools and more.";
            var shape = _shapeFactory.BuildShape(null, new object(), PageName.Home.ToString());
            return DisplayShape(shape);
          
        }
        public ActionResult bc()
        {
            return View();
        }

        public JsonResult GetTopics()
        {
            var userVerifyId = UserId.HasValue;
            var dataTopics = _itemDataService.GetChatTopics(userVerifyId);
            return Json(dataTopics, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckAccountVerification(string email, string phone)
        {
            bool result = _itemDataService.getUserverificationForChatBot(email,phone);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChatBotQuestion(string topicId)
        {
            int topicid = 0;
            int.TryParse(topicId, out topicid);
            var dataTopics = _itemDataService.GetChatQuestions(topicid);
            return Json(dataTopics, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChatBotQuestionOrAnswer(string questionId)
        {
            int questionid = 0;
            int.TryParse(questionId, out questionid);
            var dataQuestionOrAnswer = _itemDataService.GetChatQuestionsOrAnswers(questionid);
            return Json(dataQuestionOrAnswer, JsonRequestBehavior.AllowGet);
        }
    }
}