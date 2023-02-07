using System;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
namespace Apeek.Common.HttpContextImpl
{
    public class NoHttpContext : HttpContextBase
    {
        private HttpSessionStateBase _session;
        private NoHttpRequest _request;
        private NoHttpResponse _response;
        private HttpContext _context;
        public NoHttpContext(string path, bool isSession = true)
        {
            if (isSession)
                _session = new NoHttpSessionState();
            _response = NoHttpResponse.CreateNoHttpResponse();
            _request = NoHttpRequest.CreateNoHttpResponse(path);
            _request.RequestContext = new RequestContext(this, new RouteData());
            _context = new HttpContext(_request.Request, _response.Response);
        }
        public override HttpSessionStateBase Session
        {
            get { return _session; }
        }
        public override HttpRequestBase Request
        {
            get { return _request; }
        }
        public override HttpResponseBase Response
        {
            get { return _response; }
        }
        public override IPrincipal User
        {
            get { return null; }
        }
        public override object GetService(Type serviceType)
        {
            return ((IServiceProvider)this._context).GetService(serviceType);
        }
    }
}