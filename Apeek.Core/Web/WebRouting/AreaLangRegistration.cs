using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Apeek.Core.Web.WebRouting.Infrastructure;
using Apeek.Common;
using Apeek.Common.Controllers;
namespace Apeek.Core.Web.WebRouting
{
    public abstract class AreaLangRegistration : AreaRegistration
    {
        protected virtual void MapRoute(RouteCollection routeCollection, string url, RouteValueDictionary defaults = null, IRouteHandler routeHandler = null, RouteValueDictionary constraints = null, string name = "")
        {
            var t = new RouteValueDictionary();
            t.Add("area", AreaName);
            var route = new Route(url, defaults, constraints, t, routeHandler);
            routeCollection.Add(name, route);
        }
        protected void MapRoute(RouteCollection routeCollection, string url, object defaults = null, IRouteHandler routeHandler = null, object constraints = null, string name = "")
        {
            var t = new RouteValueDictionary();
            t.Add("area", AreaName);
            var route = new Route(url, ToRvd(defaults), ToRvd(constraints), t, routeHandler);
            routeCollection.Add(name, route);
        }
        protected void MapSubDomainRoute(RouteCollection routeCollection, string url, object defaults = null, IRouteHandler routeHandler = null, object constraints = null, string name = "")
        {
            var t = new RouteValueDictionary();
            t.Add("area", AreaName);
            var route = new RouteSubDomain(url, ToRvd(defaults), ToRvd(constraints), t, routeHandler);
            if (route.DataTokens == null)
            {
                route.DataTokens = new RouteValueDictionary();
            }
            route.DataTokens["__RouteName"] = name;
            routeCollection.Add(name, route);
        }
        protected void MapDomainRoute(RouteCollection routeCollection, string url, object defaults = null, IRouteHandler routeHandler = null, object constraints = null, string name = "")
        {
            var t = new RouteValueDictionary();
            t.Add("area", AreaName);
            var route = new RouteDomain(url, ToRvd(defaults), ToRvd(constraints), t, routeHandler);
            routeCollection.Add(name, route);
        }
        protected RouteValueDictionary ToRvd(object values)
        {
            return values != null ? new RouteValueDictionary(values) : new RouteValueDictionary();
        }
        protected abstract RouteCollection RegisterRoutes();
        public override void RegisterArea(AreaRegistrationContext context)
        {
            var routeCollection = RegisterRoutes();
            CreateLangRoutes(routeCollection, context.Routes);
        }
        protected void CreateLangRoutes(RouteCollection tempRouteCollection, RouteCollection originalRouteCollection)
        {
            foreach (Route r in tempRouteCollection)
            {
                if (r.RouteHandler == null)
                {
                    r.RouteHandler = new MultiCultureMvcRouteHandler();
                    var defaults = r.Defaults != null ? new RouteValueDictionary(r.Defaults) : new RouteValueDictionary();
                    var constraints = new RouteValueDictionary();
                    constraints.Add(QS.lang, new CultureConstraint(LanguageController.GetAllLanguages().Where(x => x != LanguageController.DefLanguage).Select(x => x.ToString()).ToArray()));
                    MapRoute(
                        routeCollection: originalRouteCollection,
                        url: "{lang}/" + r.Url,
                        defaults: defaults,
                        constraints: constraints,
                        routeHandler: new MultiCultureMvcRouteHandler()
                    );
                }
            }
            foreach (Route r in tempRouteCollection)
            {
                string routeName = string.Empty;
                if (r.DataTokens != null && r.DataTokens.ContainsKey("__RouteName"))
                    routeName = r.DataTokens["__RouteName"].ToString();
                originalRouteCollection.Add(routeName, r);
            }
        }
    }
}