using System.Collections.Specialized;
using System.Web;
namespace Apeek.Common.HttpContextImpl
{
    public class NoHttpRequest : HttpRequestWrapper
    {
        public HttpRequest Request { get; set; }
        private NameValueCollection _headers;
        public static NoHttpRequest CreateNoHttpResponse(string url)
        {
            return new NoHttpRequest(new HttpRequest("/", url, ""));
        }
        public NoHttpRequest(HttpRequest request) : base(request)
        {
            Request = request;
            _headers = new NameValueCollection();
            _headers.Add("HOST", request.Url.Host);
        }
        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~/"; }
        }
        public override NameValueCollection Headers
        {
            get { return _headers; }
        }
    }
}