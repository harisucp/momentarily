using System.Web.Mvc;
using Apeek.Common;
using Apeek.Web.Framework.ControllerHelpers;
using Apeek.Web.Framework.Controllers;
namespace Apeek.Web.Areas.Frontend.Controllers
{
    public class MessageController : FrontendController
    {
        private readonly MessageControllerHelper _helper;
        public MessageController()
        {
            _helper = new MessageControllerHelper();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            if (!_helper.UserHasAccess())
                return RedirectToHome();
            var result = _helper.GetMessages();
            if (result == null) return RedirectToHome();
            var shape = _shapeFactory.BuildShape(null, result, PageName.UserMessage.ToString());
            return shape != null ? DisplayShape(shape) : RedirectToHome();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Conversation(int userId)
        {
            if (!_helper.UserHasAccess())
                return RedirectToHome();
            var result = _helper.GetConversation(userId);
            if (result == null) return RedirectToHome();
            var shape = _shapeFactory.BuildShape(null, result, PageName.UserMessageConversation.ToString());
            return shape != null ? DisplayShape(shape) : RedirectToHome();
        }
    }
}