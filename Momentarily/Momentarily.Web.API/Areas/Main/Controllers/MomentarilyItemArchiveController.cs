using System.Net;
using System.Net.Http;
using System.Web.Http;
using Apeek.Web.Framework.Infrastructure;
using Momentarily.UI.Service.Services;
using Momentarily.ViewModels.Models;
namespace Momentarily.Web.API.Areas.Main.Controllers
{
    public class MomentarilyItemArchiveController : BaseApiController<MomentarilyItemSearchModel, int>
    {
        private readonly IMomentarilyItemDataService _momentarilyItemDataService;
        public MomentarilyItemArchiveController(IMomentarilyItemDataService momentarilyItemDataService)
            : base()
        {
            _momentarilyItemDataService = momentarilyItemDataService;
        }
        [HttpDelete]
        [Authorize]
        public HttpResponseMessage Delete([FromUri] int id)
        {
            var result = _momentarilyItemDataService.ArchiveGood(id, UserId.Value);
            if (result)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}