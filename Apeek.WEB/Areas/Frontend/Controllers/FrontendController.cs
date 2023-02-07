using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.Interfaces;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Core.ViewModelFactories;

namespace Apeek.Web.Areas.Frontend.Controllers
{
    public class FrontendController : BaseController
    {
        protected IUserAccessController UserAccess { get; set; }

        protected int? LocationId { get { return ContextService.LocationId; } }
        protected string LocationUrl { get; set; }

        protected int? ServiceId { get { return ContextService.ServiceId; } }
        protected string ServicePath { get; set; }
        protected string SubDomain { get; set; }

        protected int? Page { get { return ContextService.FromQueryString.Page; } }
        protected int? Count { get { return ContextService.FromQueryString.Count; } }
        protected string ReturnUrl { get { return ContextService.FromQueryString.ReturnUrl; } }
        protected bool ExternalLoginError { get { return ContextService.FromQueryString.ExternalLoginError; } }
        protected int? UserId { get { return UserAccess.UserId; } }

        public FrontendController()
        {
            UserAccess = Ioc.Get<IUserAccessController>();
        }


        protected ViewResult DisplayShape<T>(string viewName, IShape<T> shape)
        {
            ViewBag.SeoTitle = shape.SeoEntry.Title;
            ViewBag.SeoDescription = shape.SeoEntry.Description;
            ViewBag.SeoKeywords = shape.SeoEntry.Keywords;
            FillBrowseEntry(shape);

            return View(viewName, shape);
        }

        protected ViewResult DisplayShape<T>(IShape<T> shape)
        {
            ViewBag.SeoTitle = shape.SeoEntry.Title;
            ViewBag.SeoDescription = shape.SeoEntry.Description;
            ViewBag.SeoKeywords = shape.SeoEntry.Keywords;
            FillBrowseEntry(shape);

            if (string.IsNullOrWhiteSpace(shape.ViewName))
                return View(shape);

            return View(shape.ViewName, shape);
        }

        protected ViewResult DisplayShape(string viewName = null)
        {
            var seoService = new SeoEntryService();
            var seoEntry = seoService.GetSeoEntry(viewName);

            ViewBag.SeoTitle = seoEntry.Title;
            ViewBag.SeoDescription = seoEntry.Description;
            ViewBag.SeoKeywords = seoEntry.Keywords;

            var shape = new ShapeFactory(new SeoEntryService()).BuildShape<string>(viewName, null);

            return viewName != null ? View(viewName, shape) : View(shape);
        }

        protected void FillBrowseEntry<T>(IShape<T> shape)
        {
            shape.BrowseEntry.LocationId = LocationId;
            shape.BrowseEntry.LocationUrl = LocationUrl;
            shape.BrowseEntry.ServiceId = ServiceId;
            shape.BrowseEntry.ServicePath = ServicePath;
        }

        protected ActionResult AuthenticateUserWithPrivilagesAndRedirect(int userId, string redirectTo)
        {
            if (UserAccess.AuthenticateUser(userId))
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.Account, "User authenticated successfully. Redirecting ...");

                if (!string.IsNullOrWhiteSpace(redirectTo))
                    return RedirectToLocal(redirectTo);
            }
            else
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.Account,string.Format("Cannot Authenticate User: {0}", userId));
            }

            return Redirect(QuickUrl.HomeUrl());
        }

        protected ActionResult RedirectToLocal(string redirectTo)
        {
            if (Url.IsLocalUrl(redirectTo))
            {
                return Redirect(redirectTo);
            }
            else
            {
                return RedirectToHome();
            }
        }

        protected ActionResult RedirectToHome()
        {
            return Redirect(QuickUrl.HomeUrl());
        }
    }
}