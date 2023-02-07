using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Common.Controllers;
namespace Apeek.Common.UrlHelpers
{
    public class QuickUrlApi
    {
        private readonly IUrlHelper _urlHelper;
        private const string ApiByName = "ApiByName";
        private const string ApiById = "ApiById";
        private const string ApiByAction = "ApiByAction";
        public QuickUrlApi(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }
        public string ClientUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("Client"));
        }
        public string ServicesUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("Services"));
        }
        public string LocationUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("Locations"));
        }
        public string UserServiceUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("UserService"));
        }
        public string LogUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("Log"));
        }
        public string ClientReviewUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("ClientReviewApi"));
        }
        public string ChangeUserServiceUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("ChangeUserService"));
        }
        public string WriteImageUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("WriteImageUrl"));
        }
        public string ImageUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("Image"));
        }
        public string TestUrl(int? id = null)
        {
            return ApiBy("Test", id);
        }
        public string TestCustomUrl(int? id = null)
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("Test", "GetCustom", "test", id));
        }
        public string BackendServicesUrl(int? serviceId = null)
        {
            return ApiBy("BackendServices", serviceId);
        }
        public string BackendUserServicesUrl(int? serviceId = null)
        {
            return ApiBy("BackendUserServices", serviceId);
        }
        public string BackendLocationsUrl(int? locationId = null)
        {
            return ApiBy("BackendLocations", locationId);
        }
        public string NotVerifiedServicesAndLockUrl(int limit)
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("BackendServices", "GetNotVerifiedServiceAndLock", QS.limit, limit));
        }
        public string UnlockBackendUserServicesUrl()
        {
            return _urlHelper.HttpRouteUrl(ApiByAction, RouteApi("BackendServices", "GetUnlockBackendUserServices"));
        }
        private string ApiBy(string controller, int? id = null, string action = null)
        {
            string apiBy;
            if (id.HasValue)
                apiBy = ApiById;
            else apiBy = ApiByAction;
            return _urlHelper.HttpRouteUrl(apiBy, RouteApi(controller, action, "id", id));
        }
        private RouteValueDictionary RouteApi(string controller, string action = null, string paramName = null, int? paramVal = null)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", controller);
            if (!string.IsNullOrWhiteSpace(action))
                rvd.Add("action", action);
            if (!string.IsNullOrWhiteSpace(paramName) && paramVal.HasValue)
                rvd.Add(paramName, paramVal.Value);
            AddQueryStringParams(rvd);
            return rvd;
        }
        private void AddQueryStringParams(RouteValueDictionary rvd)
        {
            if (ContextService.FromQueryString.UserId.HasValue)
                rvd.Add(QS.user, ContextService.FromQueryString.UserId);
            rvd.Add(QS.lang, LanguageController.CurLang);
        }
    }
}