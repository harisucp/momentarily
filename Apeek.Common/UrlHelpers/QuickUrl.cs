using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;
using Apeek.Common.Controllers;
using Apeek.Common.HttpContextImpl;
using Apeek.Entities.Entities;
using Apeek.Entities.Web;
namespace Apeek.Common.UrlHelpers
{
    public class QuickUrl
    {
        public QuickUrlApi Api { get; set; }
        public QuickUrlBackend Backend { get; set; }
        public IUrlHelper UrlHelper { get; private set; }
        public IUrlGenerator UrlGenerator { get; set; }
        private void Init(IUrlHelper urlHelper, IUrlGenerator urlGenerator)
        {
            UrlHelper = urlHelper;
            UrlGenerator = urlGenerator;
            Api = new QuickUrlApi(UrlHelper);
            Backend = new QuickUrlBackend(UrlHelper);
        }
        public QuickUrl(MvcUrlHelper urlHelper, IUrlGenerator urlGenerator)
        {
            Init(urlHelper, urlGenerator);
        }
        public QuickUrl(HttpUrlHelper urlHelper, IUrlGenerator urlGenerator)
        {
            Init(urlHelper, urlGenerator);
        }
        #region Url
        private static string AbsoluteUrlEx(string url)
        {
            Uri originalUri = HttpContextFactory.Current.Request.Url;
            return string.Format("{0}:/{1}", originalUri.Scheme, url);
        }
        public static string GetAbsoluteUrl(string path)
        {
            Uri originalUri = HttpContextFactory.Current.Request.Url;
            return string.Format("{0}://{1}{2}", originalUri.Scheme, originalUri.Authority, path);
        }       
        public string AbsoluteUrl(string path)
        {
            return GetAbsoluteUrl(path);
        }
        public string SitemapxmlUrl()
        {
            return UrlHelper.Action("Index", "XmlSiteMap", new { page = 0 });
        }
        public string HomeUrl()
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteHome()));
        }
        public string ĀdminDashbordUrl()
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteAdminDashboard()));
        }
        public string LinkExpireUrl()
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteLinkExpire()));
        }
        public string RequestConfirmUrl()
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteRequestConfirm()));
        }
        public string LogOffUrl()
        {
            return UrlHelper.RouteUrl(RouteLogOff());
        }
        public string CreateResultUrl(CreateResult result)
        {
            return UrlHelper.RouteUrl(RouteCreateResult(result));
            //return UrlHelper.Action("CreateResult", "CreatePerson", new { result = result });
        }
        public string UserInfoUrl(int? id = null)
        {
            if (id.HasValue)
                return UrlHelper.RouteUrl(RouteEdit(id.Value));
            return UrlHelper.RouteUrl(RouteUserInfo());
        }
        public string UserPwdUrl()
        {
            return UrlHelper.RouteUrl(RouteUserPwd());
        }
        public string UserPwdUrl2()
        {
            return UrlHelper.RouteUrl(RouteUserPwd2());
        }
        public string UserProfileUrl()
        {
            return UrlHelper.RouteUrl(RouteUserProfile());
        }
        public string UserEmailUrl(List<KeyValuePair<string, string>> kvp = null)
        {
            return UrlHelper.RouteUrl(RouteUserEmail(kvp));
        }
        public string UserAccountAssociacionsUrl()
        {
            return UrlHelper.RouteUrl(RouteUserAccountAssociacions());
        }
        public string UserServicesUrl(bool? isVerified = null)
        {
            return UrlHelper.RouteUrl(RouteUserServices(isVerified));
        }
        public string UserInfoPreviewUrl(bool? isVerified = null)
        {
            return UrlHelper.RouteUrl(RouteUserInfoPreview(isVerified));
        }
        public string LocationUrl(LocationLang locationLang)
        {
            return UrlHelper.RouteUrl(RouteLocation(locationLang));
        }
        public string LocationAbsoluteUrl(LocationLang locationLang)
        {
            return AbsoluteUrlEx(LocationUrl(locationLang));
        }
        public string UserServiceUrl(string subDomain, string userServiceUrl)
        {
            return UrlHelper.RouteUrl(RouteUserService(subDomain, userServiceUrl));
        }
        public string UserServiceAbsoluteUrl(string subDomain, string userServiceUrl)
        {
            return AbsoluteUrlEx(UserServiceUrl(subDomain, userServiceUrl));
        }
        public string UserAbsoluteUrl(string userUrl, string subDomain, string locationUrl, int? locationId)
        {
            return AbsoluteUrlEx(UsereUrl(userUrl, subDomain, locationUrl, locationId));
        }
        public string UsereUrl(string userUrl, string subDomain, string locationUrl, int? locationId)
        {
            return UrlHelper.RouteUrl(RouteUser(userUrl, subDomain, locationUrl, locationId));
        }
        public string BrowseUrl(LocationLang locationLang, string serviceUrl)
        {
            return UrlHelper.RouteUrl(RouteBrowse(locationLang, serviceUrl));
        }
        public string BrowseAbsoluteUrl(LocationLang locationLang, string serviceUrl)
        {
            return AbsoluteUrlEx(UrlHelper.RouteUrl(RouteBrowse(locationLang, serviceUrl)));
        }
        public string ApiBrowseAbsoluteUrl(UserBrowseIndex userBrowseIndex)
        {
            int pageNum = userBrowseIndex.GlobalIndex / Constants.Browse.DefaultPageSize;
            if (userBrowseIndex.GlobalIndex % Constants.Browse.DefaultPageSize != 0)
                pageNum += 1;
            var rvd = RouteBrowse(userBrowseIndex.LocationLang, userBrowseIndex.ServiceLang.Path);
            RouteBrowsePage(rvd, pageNum.ToString());
            int index = userBrowseIndex.GlobalIndex % 10;
            if (index == 0)
                index = 10;
            RouteBrowseIndex(rvd, index.ToString());
            var virtualPath = GetVirtualPath("browse", rvd);
            return AbsoluteUrlEx(virtualPath);
        }
        public string BrowseUrl(ServiceLang serviceLang)
        {
            return UrlHelper.RouteUrl(RouteBrowse(serviceLang));
        }
        public string ContentUrl(string name)
        {
            return UrlHelper.RouteUrl(RouteContent(name));
        }
        public string UsersWithServiceUrl(int serviceId)
        {
            return UrlHelper.RouteUrl(RouteUsersWithService(serviceId));
        }
        public string ContactusUrl()
        {
            return UrlHelper.RouteUrl(RouteContactus());
        }
        public string CreatePersonUrl()
        {
            return UrlHelper.RouteUrl(RouteCreatePerson());
        }
        public string LoginUrl(string redirectTo, bool? externalLoginError = null)
        {
            return UrlHelper.RouteUrl(RouteLogin(redirectTo, externalLoginError));
        }
        public string RegisterUrl()
        {
            return UrlHelper.RouteUrl(RouteRegister());
        }
        public string RegisterMessageSentUrl(string email)
        {
            return UrlHelper.RouteUrl(RouteRegisterMessageSent(email));
        }
        public string RestorePwdUrl()
        {
            return UrlHelper.RouteUrl(RouteRestorePwd());
        }
        public string SiteMapUrl(string subDomain, string siteMap)
        {
            return UrlHelper.RouteUrl(RouteSiteMap(subDomain, siteMap));
        }
        public string VerifyEmail()
        {
            return UrlHelper.RouteUrl(RouteVerifyEmail());
        }
        public string VerifyIsAdmin()
        {
            return UrlHelper.RouteUrl(RouteVerifyIsAdmin());
        }
        public string UserBlocked()        {            return UrlHelper.RouteUrl(RouteBlockedUser());        }
        public string VerifyUserUrl(string verificationCode, string scheme)
        {
            return UrlHelper.Action("VerifyUser", "User", new { vc = verificationCode }, scheme);
        }

        public string AbusiveItemUrl(int goodid)
        {
            return UrlHelper.Action("Item", "Search", new { id = goodid });
        }
        public string WelcomeUserUrl(string verificationCode, string scheme)
        {
            return UrlHelper.Action("WelcomeUser", "User", new { vc = verificationCode }, scheme);
        }
        public string CreateListUrl()
        {
            return UrlHelper.Action("Create", "Listing");
        }
        public string VerifySecurityRequestUrl(string verificationCode)
        {
            var url = UrlHelper.RouteUrl(RoutVerifySecurityRequest(verificationCode));
            return AbsoluteUrl(url);
        }
        public string EditPersonEmailUrl(string createGuid, string scheme)
        {
            return UrlHelper.Action("Edit", "CreatePerson", new { t = createGuid }, scheme);
        }
        public string QuickLoginUrl(string userEmail, string verificationCode, string redirectUrl)
        {
            //return UrlHelper.Action("LoginStepTwo", "Account", new { ue=userEmail, vc = verificationCode, redirectto = redirectUrl }, scheme);
            var url = UrlHelper.RouteUrl(RoutQuickLogin(userEmail, verificationCode, redirectUrl));
            return AbsoluteUrl(url);
        }
        public string QuickLoginUrl2(string userEmail, string verificationCode, string redirectUrl)
        {
            //return UrlHelper.Action("LoginStepTwo", "Account", new { ue=userEmail, vc = verificationCode, redirectto = redirectUrl }, scheme);
            var url = UrlHelper.RouteUrl(RoutQuickLogin2(userEmail, verificationCode, redirectUrl));
            return AbsoluteUrl(url);
        }
        public string UserMessageConversationUrl(int userId)
        {
            return UrlHelper.RouteUrl(RouteUserMessageConversation(userId));
        }
        public string UserMessageConversationAbsoluteUrl(int userId)
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteUserMessageConversation(userId)));
        }
        public string SearchItemUrl(int? itemId = null)
        {
            return UrlHelper.RouteUrl(RouteSearchItem(itemId));
        }
        public string UserRequestUrl(int requestId)
        {
            return UrlHelper.RouteUrl(RouteUserRequest(requestId));
        }
        public string UserRequestAbsoluteUrl(int requestId)
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteUserRequest(requestId)));
        }
        public string GoodRequestUrl(int requestId)
        {
            return UrlHelper.RouteUrl(RouteGoodRequest(requestId));
        }
        public string GoodRequestAbsoluteUrl(int requestId)
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteGoodRequest(requestId)));
        }
        public string AdaptivePaymentConfirmAbsoluteUrl()
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteAdaptivePaymentConfirm()));
        }
        public string RequestPaymentUrl(int goodRequestId)
        {
            return UrlHelper.RouteUrl(RouteRequestPaymentAbsolute(goodRequestId));
        }
        public string RequestPaymentAbsoluteUrl(int goodRequestId)
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteRequestPaymentAbsolute(goodRequestId)));
        }
        public string DepositPaymentAbsoluteUrl(int goodRequestId)
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteDepositPaymentAbsolute(goodRequestId)));
        }
        public string ReviewSeekerAbsoluteUrl(int goodRequestId)
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteSeekerReviewAbsolute(goodRequestId)));
        }
        public string ReviewSharerAbsoluteUrl(int goodRequestId)
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteSharerReviewAbsolute(goodRequestId)));
        }
        public string DepositConfirmUrl()
        {
            return GetAbsoluteUrl(UrlHelper.RouteUrl(RouteDepositConfirm()));
        }
        #endregion Url
        #region RoutData
        public RouteValueDictionary RouteEdit(int id)
        {
            var rvd = RouteUserInfo();
            if (!rvd.ContainsKey(QS.user))
                rvd.Add(QS.user, id);
            return rvd;
        }
        public RouteValueDictionary RoutVerifySecurityRequest(string verificationCode)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "User");
            rvd.Add("action", "VerifySecurityRequest");
            rvd.Add("vc", verificationCode);
            return rvd;
        }
        public RouteValueDictionary RoutQuickLogin(string userEmail, string verificationCode, string redirectUrl)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Account");
            rvd.Add("action", "QuickLogin");
            rvd.Add("ue", userEmail);
            rvd.Add("vc", verificationCode);
            rvd.Add("redirectto", redirectUrl);
            return rvd;
        }
        public RouteValueDictionary RoutQuickLogin2(string userEmail, string verificationCode, string redirectUrl)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Account");
            rvd.Add("action", "QuickLogin2");
            rvd.Add("ue", userEmail);
            rvd.Add("vc", verificationCode);
            rvd.Add("redirectto", redirectUrl);
            return rvd;
        }
        public RouteValueDictionary RouteCreateResult(CreateResult result)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "CreatePerson");
            rvd.Add("action", "CreateResult");
            rvd.Add(QS.result, result);
            return rvd;
        }
        public RouteValueDictionary RouteUsersWithService(int serviceId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Home");
            rvd.Add("action", "UsersWithService");
            rvd.Add("serviceId", serviceId);
            return rvd;
        }
        public RouteValueDictionary RouteUserDelete(int id, string redirectTo)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "UserDelete");
            rvd.Add("id", id);
            rvd.Add(QS.returnUrl, redirectTo);
            return rvd;
        }
        public RouteValueDictionary RouteUserServices(bool? isVerified = null)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "Services");
            if (isVerified.HasValue)
                rvd.Add("verified", isVerified.Value);
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteUserInfo()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "Info");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteUserInfoPreview(bool? isVerified = null)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "UserInfoPreview");
            if (isVerified.HasValue)
                rvd.Add("verified", isVerified.Value);
            AddPermanentRouteValue(rvd);
            AddSeoRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteUserEmail(List<KeyValuePair<string, string>> kvp = null)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "UserEmail");
            if (kvp != null)
            {
                foreach (var pair in kvp)
                {
                    rvd.Add(pair.Key, pair.Value);
                }
            }
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteUserPwd()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "UserPwd");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteUserPwd2()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "ResetPassword");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteUserProfile()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "UserProfile");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteVerifyEmail()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "UserEmailConfirm");
            AddPermanentRouteValue(rvd);
            return rvd; 
        }

        public RouteValueDictionary RouteVerifyIsAdmin()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Admin");
            rvd.Add("action", "Index");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteBlockedUser()        {            var rvd = new RouteValueDictionary();            rvd.Add("area", "Frontend");            rvd.Add("controller", "User");            rvd.Add("action", "BlockedUser");            AddPermanentRouteValue(rvd);            return rvd;        }
        public RouteValueDictionary RouteUserAccountAssociacions()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "User");
            rvd.Add("action", "UserAccountAssociations");
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteHome()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Home");
            rvd.Add("action", "Index");
            return rvd;
        }

        public RouteValueDictionary RouteAdminDashboard()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "AdminDashboard");
            rvd.Add("action", "Index");
            return rvd;
        }

        public RouteValueDictionary RouteLinkExpire()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Home");
            rvd.Add("action", "LinkExpire");
            return rvd;
        }
        public RouteValueDictionary RouteRequestConfirm()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Payment");
            rvd.Add("action", "BookingRequestConfirm");
            return rvd;
        }
        public RouteValueDictionary RouteDepositConfirm()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Payment");
            rvd.Add("action", "BookingDepositConfirm");
            return rvd;
        }
        public object RouteContactus()
        {
            return new { area = "Frontend", controller = "Home", action = "ContactUs" };
        }
        public object RouteContent(string name)
        {
            return new { area = "Frontend", controller = "Content", action = "Content", url = name };
        }
        public object RouteCreatePerson()
        {
            return new { area = "Frontend", controller = "CreatePerson", action = "Create" };
        }
        public RouteValueDictionary RouteLocation(LocationLang locationLang)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Home");
            rvd.Add("action", "Location");
            rvd.Add(QS.subDomain, locationLang.SubDomainUrl);
            if (string.Compare(locationLang.SubDomainUrl, locationLang.Url, StringComparison.OrdinalIgnoreCase) != 0)
                rvd.Add(QS.location, UrlGenerator.GetUrl(locationLang.Location.Id, locationLang.Url));
            AddPermanentRouteValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteSiteMap(string subDomain, string siteMap)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Sitemap");
            rvd.Add("action", "Index");
            rvd.Add(QS.subDomain, subDomain);
            rvd.Add(QS.sitemap, siteMap);
            return rvd;
        }
        public RouteValueDictionary RouteBrowse(ServiceLang serviceLang)
        {
            var rvd = new RouteValueDictionary();
            //rvd.Add("area", "Frontend");
            rvd.Add("controller", "Person");
            rvd.Add("action", "PersonList");
            rvd.Add(QS.location, ContextService.FromQueryString.LocationUrl);
            //browse to location page
            if (serviceLang == null)
                rvd.Add("service", null);
            else
                rvd.Add("service", serviceLang.Path);
            //AddPermanentRouteValue(rvd);
            AddPermanentBrowseValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteBrowse(LocationLang locationLang, string servicePath)
        {
            var rvd = new RouteValueDictionary();
            //rvd.Add("area", "Frontend");
            rvd.Add("controller", "Person");
            rvd.Add("action", "PersonList");
            rvd.Add("service", servicePath);
            rvd.Add(QS.subDomain, locationLang.SubDomainUrl);
            if (string.Compare(locationLang.SubDomainUrl, locationLang.Url, StringComparison.OrdinalIgnoreCase) != 0)
                rvd.Add(QS.location, UrlGenerator.GetUrl(locationLang.Location.Id, locationLang.Url));
            //AddPermanentRouteValue(rvd);
            AddPermanentBrowseValue(rvd);
            return rvd;
        }
        private void AddPermanentRouteValue(RouteValueDictionary rvd)
        {
            if (ContextService.FromQueryString.UserId.HasValue)
                rvd.Add(QS.user, ContextService.FromQueryString.UserId);
        }
        private void AddSeoRouteValue(RouteValueDictionary rvd)
        {
            if (!string.IsNullOrWhiteSpace(ContextService.FromQueryString.UtmMedium))
            {
                rvd.Add(QS.utmMedium, ContextService.FromQueryString.UtmMedium);
            }
            if (!string.IsNullOrWhiteSpace(ContextService.FromQueryString.UtmSource))
            {
                rvd.Add(QS.utmSource, ContextService.FromQueryString.UtmSource);
            }
            if (!string.IsNullOrWhiteSpace(ContextService.FromQueryString.UtmCampaign))
            {
                rvd.Add(QS.utmCampaign, ContextService.FromQueryString.UtmCampaign);
            }
        }
        private void AddPermanentBrowseValue(RouteValueDictionary rvd)
        {
            if (ContextService.FromQueryString.OrderBy.HasValue)
                rvd.Add(QS.orderBy, (int)ContextService.FromQueryString.OrderBy);
        }
        public RouteValueDictionary RouteBrowsePage(string pageNum)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Person");
            rvd.Add("action", "PersonList");
            RouteBrowsePage(rvd, pageNum);
            //AddPermanentRouteValue(rvd);
            AddPermanentBrowseValue(rvd);
            return rvd;
        }
        public RouteValueDictionary RouteBrowsePage(RouteValueDictionary rvd, string pageNum)
        {
            rvd.Add(QS.page, pageNum);
            return rvd;
        }
        public RouteValueDictionary RouteBrowseIndex(RouteValueDictionary rvd, string browseIndex)
        {
            rvd.Add(QS.index, browseIndex);
            return rvd;
        }
        public RouteValueDictionary RouteBrowseOrder(OrderBy orderBy)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Person");
            rvd.Add("action", "PersonList");
            rvd.Add(QS.orderBy, (int)orderBy);
            return rvd;
        }
        public RouteValueDictionary RouteLogOff()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Account");
            rvd.Add("action", "LogOff");
            return rvd;
        }
        public RouteValueDictionary RouteMyBrowse()
        {
            if (ContextService.AuthenticatedUser == null)
            {
                return RouteHome();
            }
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Person");
            rvd.Add("action", "PersonList");
            rvd.Add("location", 2);
            rvd.Add("service", 1);
            rvd.Add(QS.user, ContextService.AuthenticatedUser.UserId);
            return rvd;
        }
        public RouteValueDictionary RouteRestorePwd()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Account");
            rvd.Add("action", "RestorePwd");
            return rvd;
        }
        public RouteValueDictionary RouteLogin(string redirectTo, bool? externalLoginError = null)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Account");
            rvd.Add("action", "Login");
            if (!string.IsNullOrWhiteSpace(redirectTo))
                rvd.Add(QS.returnUrl, redirectTo);
            if (externalLoginError.HasValue)
                rvd.Add(QS.externalLoginError, externalLoginError.Value);
            return rvd;
        }

        //public RouteValueDictionary RouteAdmin(string redirectTo, bool? externalLoginError = null)
        //{
        //    var rvd = new RouteValueDictionary();
        //    rvd.Add("area", "Frontend");
        //    rvd.Add("controller", "Admin");
        //    rvd.Add("action", "Index");
        //    if (!string.IsNullOrWhiteSpace(redirectTo))
        //        rvd.Add(QS.returnUrl, redirectTo);
        //    if (externalLoginError.HasValue)
        //        rvd.Add(QS.externalLoginError, externalLoginError.Value);
        //    return rvd;
        //}

        public RouteValueDictionary RouteRegister()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Account");
            rvd.Add("action", "Register");
            return rvd;
        }
        public RouteValueDictionary RouteNotFound(string url)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Error");
            rvd.Add("action", "NotFound");
            rvd.Add("url", url);
            return rvd;
        }
        public RouteValueDictionary RouteUserService(string subDomain, string userServiceUrl)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Home");
            rvd.Add("action", "Service");
            rvd.Add("userService", userServiceUrl);
            if (!string.IsNullOrWhiteSpace(subDomain))
                rvd.Add(QS.subDomain, subDomain);
            return rvd;
        }
        public RouteValueDictionary RouteUser(string userUrl, string subDomain, string locationUrl, int? locationId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Home");
            rvd.Add("action", "User");
            rvd.Add("userUrl", userUrl);
            rvd.Add(QS.subDomain, subDomain);
            if (string.Compare(subDomain, locationUrl, StringComparison.OrdinalIgnoreCase) != 0 && locationId.HasValue)
                rvd.Add(QS.location, UrlGenerator.GetUrl(locationId.Value, locationUrl));
            return rvd;
        }
        public RouteValueDictionary RouteExternalLogin(string provider, string redirectTo)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Account");
            rvd.Add("action", "ExternalLogin");
            rvd.Add("provider", provider);
            rvd.Add(QS.returnUrl, redirectTo);
            return rvd;
        }
        public RouteValueDictionary RouteRegisterMessageSent(string email)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Account");
            rvd.Add("action", "RegisterMessageSent");
            rvd.Add("email", email);
            return rvd;
        }
        public RouteValueDictionary RouteUserMessageConversation(int userId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Message");
            rvd.Add("action", "Conversation");
            rvd.Add("userId", userId);
            return rvd;
        }
        public RouteValueDictionary RouteSearchItem(int? itemId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Search");
            rvd.Add("action", "Item");
            rvd.Add("id", itemId);
            return rvd;
        }
        public RouteValueDictionary RouteUserRequest(int requestId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Booking");
            rvd.Add("action", "Request");
            rvd.Add("id", requestId);
            return rvd;
        }
        public RouteValueDictionary RouteGoodRequest(int requestId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Listing");
            rvd.Add("action", "Booking");
            rvd.Add("id", requestId);
            return rvd;
        }
        public RouteValueDictionary RouteAdaptivePaymentConfirm()
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("area", "Frontend");
            rvd.Add("controller", "Payment");
            rvd.Add("action", "AdaptivePaymentConfirm");
            return rvd;
        }
        public RouteValueDictionary RouteRequestPaymentAbsolute(int goodRequestId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Payment");
            rvd.Add("action", "Pay");
            rvd.Add("goodRequestId", goodRequestId);
            return rvd;
        }
        public RouteValueDictionary RouteDepositPaymentAbsolute(int goodRequestId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Payment");
            rvd.Add("action", "GetBookingDepositPayment");
            rvd.Add("goodRequestId", goodRequestId);
            return rvd;
        }
        public RouteValueDictionary RouteSeekerReviewAbsolute(int goodRequestId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Booking");
            rvd.Add("action", "BookingReview");
            rvd.Add("id", goodRequestId);
            return rvd;
        }
        public RouteValueDictionary RouteSharerReviewAbsolute(int goodRequestId)
        {
            var rvd = new RouteValueDictionary();
            rvd.Add("controller", "Listing");
            rvd.Add("action", "BookingReview");
            rvd.Add("id", goodRequestId);
            return rvd;
        }
        #endregion RoutData
        private string GetVirtualPath(string routeName, IDictionary<string, object> routeValues)
        {
            if (routeValues == null)
            {
                routeValues = new HttpRouteValueDictionary();
                routeValues.Add("httproute", true);
            }
            else
            {
                routeValues = new HttpRouteValueDictionary(routeValues);
            }
            HttpConfiguration configuration = HttpRequestMessageExtensions.GetConfiguration(UrlHelper.Request);
            if (configuration == null)
                throw new ApeekException("No Configuration");
            IHttpVirtualPathData virtualPath = configuration.Routes.GetVirtualPath(UrlHelper.Request, routeName, routeValues);
            if (virtualPath == null)
                return null;
            else
                return virtualPath.VirtualPath;
        }
    }
}