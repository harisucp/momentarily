using Apeek.Common;
using Apeek.Core.Services;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.Controllers;
using CaptchaMvc.HtmlHelpers;
using Momentarily.UI.Service.Services;
using System.Web.Mvc;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class SendHelpController : FrontendController
    {

        private readonly ISendMessageService _sendMessageService;
        private readonly ISettingsDataService _settingsDataService;
        private readonly IMomentarilyItemTypeService _typeService;
        public SendHelpController( ISendMessageService sendMessageService, ISettingsDataService settingsDataService, IMomentarilyItemTypeService typeService)
        {
            _sendMessageService = sendMessageService;
            _settingsDataService = settingsDataService;
            _typeService = typeService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Contact Us | momentarily";
            ViewBag.Description = "Contact us for issues regarding using the momentarily platform.";
            var model = new ContactUsEntry();

            var shape = _shapeFactory.BuildShape(null, model, PageName.Home.ToString());
            return DisplayShape(shape);
        }
        [HttpPost]
        public ActionResult Index(Shape<ContactUsEntry> shape)
        {
            if (!this.IsCaptchaValid("Validate your captcha"))
            {
                ViewBag.ErrorMessage = "Invalid Captcha";
                return DisplayShape(shape);
            }

            if (ModelState.IsValid)
            {
                var result = _sendMessageService.SendEmailContactUs(shape.ViewModel);

                shape.IsError = !result;
                if (result)
                {
                    shape.ViewModel = new ContactUsEntry();
                    ViewBag.IsEmailSent = "True";
                }
                else
                {
                    ViewBag.IsEmailSent = "False";
                }
            }
            return DisplayShape(shape);
        }


    }
}

