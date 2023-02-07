using System.Net;
using System.Net.Http;
using Apeek.Common;
using Apeek.Web.Framework.Infrastructure;
using Momentarily.UI.Service.Services;
using Momentarily.ViewModels.Models;
namespace Momentarily.Web.API.Areas.Main.Controllers
{
    public class MomentarilyItemMapController : BaseApiController<MomentarilyItemMapViewModel, int>
    {
        private readonly IMomentarilyItemDataService _itemDataService;
        public MomentarilyItemMapController(IMomentarilyItemDataService itemDataService)
        {
            _itemDataService = itemDataService;
        }
        public HttpResponseMessage Post(MomentarilyItemSearchModel searchModel)
        {
            if(searchModel.Location==null)
            {
                searchModel.Location = "";
            }
            searchModel.Location = searchModel.Location.Replace("'", "''");
            var result = _itemDataService.GetFilteredItems(searchModel);
            if (result.CreateResult == CreateResult.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result.Obj);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}