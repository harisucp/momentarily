using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Common;
using Moq;
namespace Apeek.Test.Common.Utils
{
    public class TestAspMvcController
    {
        public static T CreateApi<T>() where T : ApiController
        {
            var ctrl = Ioc.Get<T>();
            ctrl.Request = new HttpRequestMessage();
            //ctrl.Request.SetConfiguration(new HttpConfiguration());
            return ctrl;
        }
        public static T Create<T>() where T : Controller
        {
            var request = new Mock<HttpRequestBase>();
            // Not working - IsAjaxRequest() is static extension method and cannot be mocked
            // request.Setup(x => x.IsAjaxRequest()).Returns(true /* or false */);
            // use this
            request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection {{"X-Requested-With", "XMLHttpRequest"}});
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            var ctrl = Ioc.Get<T>();
            ctrl.ControllerContext = new ControllerContext(context.Object, new RouteData(), ctrl);
            return ctrl;
        }
    }
}