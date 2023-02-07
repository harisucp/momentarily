using System.IO;
using System.Web;
namespace Apeek.Common.HttpContextImpl
{
    public class NoHttpResponse : HttpResponseWrapper
    {
        public HttpResponse Response { get; set; }
        public static NoHttpResponse CreateNoHttpResponse()
        {
            return new NoHttpResponse(new HttpResponse(new StringWriter()));
        }
        private NoHttpResponse(HttpResponse response)
            : base(response)
        {
            Response = response;
        }
        public override string ApplyAppPathModifier(string virtualPath)
        {
            return virtualPath;
        } 
    }
}