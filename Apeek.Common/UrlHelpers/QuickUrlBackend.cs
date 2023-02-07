using System.Web.Routing;
namespace Apeek.Common.UrlHelpers
{
    public class QuickUrlBackend
    {
        private readonly IUrlHelper _urlHelper;
        public QuickUrlBackend(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }
        public string ClearCacheUrl()
        {
            return _urlHelper.RouteUrl(RouteClearCache());
        }
        public string ClearSiteMapUrl()
        {
            return _urlHelper.RouteUrl(RouteClearSiteMap());
        }
        public string EmailTemplatesUrl()
        {
            return _urlHelper.RouteUrl(RouteEmailTemplates());
        }
        public string LocationsUrl()
        {
            return _urlHelper.RouteUrl(RouteLocations());
        }
        public string ServicesUrl()
        {
            return _urlHelper.RouteUrl(RouteServices());
        }
        public string ReportsUrl()
        {
            return _urlHelper.RouteUrl(RouteReports());
        }
        public string ReportsSaveUrl()
        {
            return _urlHelper.RouteUrl(RouteReportsSave());
        }
        public string ReportsReportDataUrl()
        {
            return _urlHelper.RouteUrl(RouteReportsReportData());
        }
        public string BackendUrl()
        {
            return _urlHelper.RouteUrl(RouteBackend());
        }
        public string ReportsComponentDataUrl()
        {
            return _urlHelper.RouteUrl(RouteReportsComponentData());
        }
        private RouteValueDictionary RouteEmailTemplates()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "EmailTemplates");
            rvd.Add("action", "NotificationServiceChanged");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        private RouteValueDictionary RouteClearSiteMap()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Cache");
            rvd.Add("action", "ClearSitemap");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        private RouteValueDictionary RouteClearCache()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Cache");
            rvd.Add("action", "Clear");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        private RouteValueDictionary RouteBackend()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Default");
            rvd.Add("action", "Index");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteReportsSave()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Reports");
            rvd.Add("action", "Save");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteReportsComponentData()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Reports");
            rvd.Add("action", "GetComponentData");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteReportsReportData()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Reports");
            rvd.Add("action", "GetReportData");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteReports()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Reports");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteServices()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Default");
            rvd.Add("action", "Services");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteService(int serviceId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Default");
            rvd.Add("action", "Service");
            AddPermanentRouteValue(rvd, serviceId);
            return rvd;
        }
        public RouteValueDictionary RouteLocations()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Default");
            rvd.Add("action", "Locations");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteLocation(int locationId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Default");
            rvd.Add("action", "Location");
            AddPermanentRouteValue(rvd, locationId);
            return rvd;
        }
        public RouteValueDictionary RouteLocationServiceContent(int locationId, int serviceId, string redirectTo)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Default");
            rvd.Add("action", "LocationServiceContent");
            rvd.Add(QS.locationid, locationId);
            rvd.Add(QS.serviceid, serviceId);
            rvd.Add(QS.returnUrl, redirectTo);
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        private void AddPermanentRouteValue(RouteValueDictionary rvd, int? id = null)
        {
            rvd.Add("Area", "Backend");
            if(id.HasValue)
                rvd.Add("id", id);
        }
    }
}