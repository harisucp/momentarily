using Apeek.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Apeek.Common;
using Apeek.Core.Services;
using Apeek.ViewModels.Models.Impl;
using Momentarily.Entities.Entities;
namespace Momentarily.Web.API.Areas.Main.Controllers
{
    public class MomentarilySeekerReviewController : BaseApiController<ListViewModel<ReviewViewModel>, int>
    {
        private readonly IUserDataService<MomentarilyItem> _userService;
        public MomentarilySeekerReviewController(IUserDataService<MomentarilyItem> userService)
        {
            _userService = userService;
        }
        public HttpResponseMessage Get(int userId, int page)
        {
            var result = _userService.GetSeekersReview(userId, page);
            if (result.CreateResult == CreateResult.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result.Obj);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
