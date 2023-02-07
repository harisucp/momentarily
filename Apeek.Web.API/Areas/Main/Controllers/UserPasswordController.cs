using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.Infrastructure;
using Apeek.Web.Infrastructure;
namespace Apeek.Web.API.Areas.Main.Controllers
{
    public class UserPasswordController : BaseApiController<UserPwdModel, int>
    {
        public IAccountDataService UserDataService;
        private readonly ISendMessageService _sendMessageService;
        public UserPasswordController(IAccountDataService userDataService, ISendMessageService sendMessageService)
        {
            UserDataService = userDataService;
            _sendMessageService = sendMessageService;
        }
        [Authorize]
        public HttpResponseMessage Put(UserPwdModel model)
        {
            if (!UserId.HasValue || !UserAccessController.HasAccess(Privileges.CanEditUsers, UserId))
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            if (model != null && ModelState.IsValid)
            {
                var result = UserDataService.SetUserPwd(model, UserId.Value);
                if (result.CreateResult == CreateResult.Success)
                {
                    var user = result.Obj;
                    var messageResult = _sendMessageService.SendEmailChangePasswordMessage(user.Email,user.FirstName);
                    if (messageResult)
                    {
                        Ioc.Get<IDbLogger>().LogMessage(LogSource.User, string.Format("Change password mail was sent to address: email {0}; user id: {1}.", user.Email, user.Id));
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    Ioc.Get<IDbLogger>().LogError(LogSource.UserController, string.Format("Cannot send change password email when creating user: userid={0}", user.Id));
                }
                Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Update User Pwd: {0}", model));
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}