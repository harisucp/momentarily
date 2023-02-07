using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Apeek.Common.HttpContextImpl;
using Apeek.Core.Services;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.Web.API.Areas.Main.Controllers;
using Momentarily.Common.Definitions;
namespace Momentarily.Web.API.Areas.Main.Controllers
{
    public class MomentarilyGoodImageController : ImageController
    {
        public MomentarilyGoodImageController(IImageDataService imageDataService)
            : base(imageDataService)
        {
        }
        [Authorize]
        [HttpPost]
        public HttpResponseMessage Post()
        {
            return PostOriginalImage(ImageFolder.Good);
        }
        [Authorize]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            if (id == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            return Delete(id, ImageFolder.Good);
        }
    }
}