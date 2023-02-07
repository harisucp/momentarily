using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace Apeek.Web.API.Areas.Main.Controllers
{
    public class TestController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Running");
        }
    }
}