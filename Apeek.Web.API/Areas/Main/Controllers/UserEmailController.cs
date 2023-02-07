using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models;
using Apeek.Web.Framework.Infrastructure;
using Apeek.Web.Infrastructure;
namespace Apeek.Web.API.Areas.Main.Controllers
{
    public class UserEmailController : BaseApiController<UserEmailModel, int>
    {
        public IAccountDataService UserDataService;
        public UserEmailController(IAccountDataService userDataService)
        {
            UserDataService = userDataService;
        }
        [Authorize]
        public HttpResponseMessage Put(UserEmailModel model)
        {
            var result = new WebApiResult<User>(CreateResult.EmailChangeError, new User());
            if (!UserId.HasValue || !UserAccessController.HasAccess(Privileges.CanEditUsers, UserId))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (model != null && ModelState.IsValid)
            {
                if (string.Compare(model.OldUserEmail, model.UserEmail,
                        StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                var userData = UserDataService.GetUser(UserId.Value);
                var user = UserDataService.GetUserByEmail(model.UserEmail);
                if (user == null)
                {
                    var request =
                            UserDataService.CreateUserSecurityDataChangeRequest(
                                UserSecurityDataType.Email, model.UserEmail, model.OldUserEmail, UserId.Value);
                    if (request != null)
                    {
                        var sms = Ioc.Get<ISendMessageService>();
                        if (sms.SendEmailVerifySecurityDataMessage(request, userData.FirstName + " " + userData.LastName))
                        {
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Send Email VerifySecurityData  Email: {0}; User: {1}", model.UserEmail, UserId.Value));
                    }
                    else
                    {
                        Ioc.Get<IDbLogger>().LogError(LogSource.UserService, string.Format("Cannot Update Email: {0} for User: {1}", model.UserEmail, UserId.Value));
                    }
                }
                else
                {
                    result.Message = "This email already exists";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, result);
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}