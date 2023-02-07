using System.Collections.Generic;
using System.IO;
using System.Web;
namespace Apeek.Common.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string ParamsToString(this HttpRequestBase request)
        {
            request.InputStream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(request.InputStream).ReadToEnd();
        }
        public static Dictionary<string, string> ToRaw(this HttpRequest request)
        {
            return new HttpRequestWrapper(request).ToRaw();
        }
        public static Dictionary<string, string> ToRaw(this HttpRequestBase requestBase)
        {
            Dictionary<string, string> writer = new Dictionary<string, string>();
            WriteStartLine(requestBase, ref writer);
            WriteHeaders(requestBase, ref writer);
            WriteBody(requestBase, ref writer);
            return writer;
        }
        private static void WriteStartLine(HttpRequestBase request, ref Dictionary<string, string> writer)
        {
            writer.Add("HttpMethod", request.HttpMethod);
            writer.Add("Url", request.Url.ToString());
            writer.Add("ServerVariables", request.ServerVariables["SERVER_PROTOCOL"]);
        }
        private static void WriteHeaders(HttpRequestBase request, ref Dictionary<string, string> writer)
        {
            foreach (string key in request.Headers.AllKeys)
            {
                writer.Add(key, request.Headers[key]);
            }
        }
        private static void WriteBody(HttpRequestBase request, ref Dictionary<string, string> writer)
        {
            StreamReader reader = new StreamReader(request.InputStream);
            try
            {
                string body = reader.ReadToEnd();
                writer.Add("Body", body);
            }
            finally
            {
                reader.BaseStream.Position = 0;
            }
        }
    } 
}