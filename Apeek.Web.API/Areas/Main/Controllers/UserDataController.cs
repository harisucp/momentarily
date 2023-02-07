using Apeek.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Apeek.Common;
using Apeek.Core.Services;
using Apeek.ViewModels.Models.Impl;
namespace Apeek.Web.API.Areas.Main.Controllers
{
    public class UserDataController : BaseApiController<UserDataViewModel, int>
    {
        protected IUserMessageService _userMessageService;
        public UserDataController(IUserMessageService userMessageService)
        {
            _userMessageService = userMessageService;
        }
        public HttpResponseMessage Get()
        {
            UserDataViewModel model = new UserDataViewModel();
            if (UserId.HasValue)
            {
                var result = _userMessageService.GetUnreadMessageCount(UserId.Value);
                model.UnreadMessagesCount = result;
                model.Id = UserId.Value;
            }
            return Request.CreateResponse(HttpStatusCode.OK, model);
        }
    }
}