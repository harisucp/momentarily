using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.HttpContextImpl;
using Apeek.Core.IocRegistry;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.Test.Common.Utils;
using Apeek.Web.DependencyResolution;
using Moq;
using NUnit.Framework;
namespace Apeek.Test.Common
{
    public class BaseTest
    {
        private readonly bool _useProfiler;
        public static Language DefLanguage { get { return Language.en; } }
        public BaseTest(bool useProfiler = false)
        {
            HttpContextFactory.SetCurrentContext(GetMockedHttpContext());
            _useProfiler = useProfiler;
        }
        [TestFixtureSetUp]
        public virtual void SetUp()
        {
            Ioc.Add(x =>
            {
                x.AddRegistry<DefaultRegistry>();
                x.AddRegistry(new ApeekSingletonRegistry());
                x.AddRegistry(new IocScanerRegistry(AppDomain.CurrentDomain.BaseDirectory, new string[] { "Apeek" }));
                x.For<IDbFluentConfigurationManager>().Use<DbFluentConfigurationManagerForTests>();
            });
            Ioc.Add(x =>
            {
                x.AddRegistry(new CoreIocRegistry());
            });
            //if(_useProfiler)
                //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
        }
        [TestFixtureTearDown]
        public virtual void TearDown()
        {
            //if (_useProfiler)
                //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Stop();
        }
        public static void SetAuthenticatedContext(IPrincipal principal)
        {
            HttpContextFactory.SetCurrentContext(GetMockedHttpContext(principal));
        }
        public static HttpContextBase GetMockedHttpContext(IPrincipal principal = null)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();
            var urlHelper = new Mock<UrlHelper>();
            var serverVariables = new NameValueCollection();
            serverVariables.Add("REMOTE_ADDR", "test");
            var queryString = new NameValueCollection();
            queryString.Add(QS.lang, DefLanguage.ToString());
            request.SetupGet(x => x.ServerVariables).Returns(serverVariables);
            request.SetupGet(x => x.QueryString).Returns(queryString);
            //var routes = new RouteCollection();
            //MvcApplication.RegisterModule();.RegisterRoutes(routes);
            session.Setup(x => x[QS.lang]).Returns(DefLanguage);
            var requestContext = new Mock<RequestContext>();
            requestContext.Setup(x => x.HttpContext).Returns(context.Object);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(principal!=null ? principal : user.Object);
            server.Setup(x => x.MapPath(It.IsAny<String>())).Returns((string s) => AppSettings.GetInstance().BaseDirectory + s);
            //user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            //identity.Setup(id => id.IsAuthenticated).Returns(true);
            //identity.Setup(id => id.Name).Returns("test");
            request.Setup(req => req.Url).Returns(new Uri("http://www.google.com"));
            request.Setup(req => req.RequestContext).Returns(requestContext.Object);
            requestContext.Setup(x => x.RouteData).Returns(new RouteData());
            return context.Object;
        }
    }
}