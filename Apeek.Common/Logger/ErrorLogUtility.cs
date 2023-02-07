using System;
using System.Web;
using Apeek.Common.Extensions;
using Apeek.Entities.Web;
namespace Apeek.Common.Logger
{
    public class ErrorLogUtility
    {
        public readonly IApeekPrincipal User;
        public readonly HttpRequestBase RequestBase;
        public ErrorLogUtility(HttpRequestBase requestBase, IApeekPrincipal user)
        {
            User = user;
            if (requestBase == null)
            {
                throw new NullReferenceException("requestBase is null at ErrorLogUtility.");
            }
            RequestBase = requestBase;
        }
        public ErrorLogUtility(HttpRequest request, ApeekPrincipal user)
            : this(new HttpRequestWrapper(request), user)
        {
        }
        private ErrorModel GetErrorModel(string message)
        {
            var errorModel = new ErrorModel();
            SetRouteDetail(ref errorModel);
            SetUserDetail(ref errorModel);
            SetRequestDetail(ref errorModel);
            SetExceptionDetail(ref errorModel, message);
            return errorModel;
        }
        private void SetUserDetail(ref ErrorModel errorModel)
        {
            errorModel.User = User;
        }
        public ErrorModel GetErrorModel(string message, object session)
        {
            var errorModel = GetErrorModel(message);
            SetSessionDetail(ref errorModel, session);
            return errorModel;
        }
        private void SetRequestDetail(ref ErrorModel errorModel)
        {
            errorModel.RequestHeader = RequestBase.ToRaw();
            errorModel.RequestData = RequestBase.ParamsToString();
            errorModel.RequestingIp = RequestBase.ServerVariables["REMOTE_ADDR"];
            errorModel.RequestingUrl = RequestBase.Url.ToString();
        }
        private void SetRouteDetail(ref ErrorModel errorModel)
        {
            Func<string, string> routeDataValue = delegate(string indexName)
            {
                bool hasValue = RequestBase.RequestContext.RouteData.Values[indexName] != null;
                return hasValue ? RequestBase.RequestContext.RouteData.Values[indexName].ToString() : "";
            };
            errorModel.ControllerName = routeDataValue("controller");
            errorModel.ActionName = routeDataValue("action");
        }
        private void SetExceptionDetail(ref ErrorModel errorModel, string message)
        {
            errorModel.Message = message;
            errorModel.CreatedDateTime = DateTime.Now;
        }
        private void SetSessionDetail(ref ErrorModel errorModel, object session)
        {
            errorModel.HasSession = session != null;
            errorModel.Session = (session != null) ? session.ToString() : "";
        }
    }
}