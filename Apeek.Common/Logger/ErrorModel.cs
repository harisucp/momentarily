using System;
using System.Collections.Generic;
using Apeek.Common.Extensions;
using Apeek.Entities.Web;
namespace Apeek.Common.Logger
{
    public class ErrorModel
    {
        public string RequestingUrl { get; set; }
        public string Message { get; set; }
        /*RouteDetail*/
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        /*SessionDetail*/
        public bool? HasSession { get; set; }
        public string Session { get; set; }
        /*RequestDetail*/
        public Dictionary<string, string> RequestHeader { get; set; }
        public string RequestData { get; set; }
        public string RequestingIp { get; set; }
        /*ExceptionDetail*/
        public DateTime CreatedDateTime { get; set; }
        public IApeekPrincipal User { get; set; }
        public override string ToString()
        {
            return System.Web.HttpUtility.UrlDecode(JsonHelper.Serialize(this));
        }
    }
}