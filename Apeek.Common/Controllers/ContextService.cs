using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Apeek.Common.Configuration;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.Entities.Web;
namespace Apeek.Common.Controllers
{
    public class FromQueryString
    {
        public string SearchTerm { get; set; }
        public Language Lang { get; set; }
        public int? LocationId { get; set; }
        public int LangId { get; set; }
        public int? Page { get; set; }
        public int? UserId { get; set; }
        public string ReturnUrl { get; set; }
        public OrderBy? OrderBy { get; set; }
    }
    public class Context
    {
        public int? ServiceId { get; set; }
        public int? LocationId { get; set; }
        public ApeekPrincipal AuthenticatedUser { get; set; }
        public bool IsUserAuthenticated { get; set; }
        public FromQueryString FromQueryString { get; set; }
        public Language CurLang { get; set; }
        public int CurLangId { get; set; }
        public Context()
        {
            FromQueryString = new FromQueryString();
        }
    }
    public class ContextService
    {
        private static Context _context;
        public static string ImgFileServerUrl;
        public static Context Context
        {
            get
            {
                //if (_context == null)
                {
                    _context = new Context()
                    {
                        ServiceId = ServiceId,
                        LocationId = LocationId,
                        CurLang = LanguageController.CurLang,
                        CurLangId = LanguageController.CurLangId,
                        AuthenticatedUser = AuthenticatedUser,
                        IsUserAuthenticated = IsUserAuthenticated
                    };
                    _context.FromQueryString.OrderBy = FromQueryString.OrderBy;
                    _context.FromQueryString.SearchTerm = FromQueryString.SearchTerm;
                    _context.FromQueryString.Lang = FromQueryString.Lang;
                    _context.FromQueryString.LangId = FromQueryString.LangId;
                    _context.FromQueryString.Page = FromQueryString.Page;
                    _context.FromQueryString.UserId = FromQueryString.UserId;
                    _context.FromQueryString.ReturnUrl = FromQueryString.ReturnUrl;
                    _context.FromQueryString.LocationId = FromQueryString.LocationId;
                }
                return _context;
            }
        }
        private static HttpRequestBase _request;
        public static class FromQueryString
        {
            public static string SearchTerm
            {
                get { return _request != null ? _request.Form[QS.SearchTerm] : null; }
            }
            public static Language Lang
            {
                get { return LanguageController.GetLanguage(HttpContextFactory.Current.Request.QueryString[QS.lang]); }
            }
            public static int LangId
            {
                get { return (int)LanguageController.GetLanguage(HttpContextFactory.Current.Request.QueryString[QS.lang]); }
            }
            public static int? Page
            {
                get { return Converter.GetInt(HttpContextFactory.Current.Request.QueryString[QS.page]); }
            }
            public static int? Count
            {
                get { return Converter.GetInt(HttpContextFactory.Current.Request.QueryString[QS.count]); }
            }
            public static int? LocationId
            {
                get { return new UrlGenerator().GetId(HttpContextFactory.Current.Request.QueryString[QS.location]); }
            }
            public static string LocationUrl
            {
                get { return HttpContextFactory.Current.Request.QueryString[QS.location]; }
            }
            public static string UtmSource
            {
                get { return HttpContextFactory.Current.Request.QueryString[QS.utmSource]; }
            }
            public static string UtmMedium
            {
                get { return HttpContextFactory.Current.Request.QueryString[QS.utmMedium]; }
            }
            public static string UtmCampaign
            {
                get { return HttpContextFactory.Current.Request.QueryString[QS.utmCampaign]; }
            }
            public static OrderBy? OrderBy
            {
                get
                {
                    var value = Converter.GetInt(HttpContextFactory.Current.Request.QueryString[QS.orderBy]);
                    if (value == null)
                        return null;
                    OrderBy orderBy;
                    if (Enum.TryParse(value.ToString(), true, out orderBy))
                    {
                        return orderBy;
                    }
                    return Entities.Web.OrderBy.DefBrowseOrder;
                }
            }
            public static int? UserId
            {
                get { return Converter.GetInt(HttpContextFactory.Current.Request.QueryString[QS.user]); }
            }
            public static string ReturnUrl
            {
                get { return HttpContextFactory.Current.Request.QueryString[QS.returnUrl]; }
            }
            public static bool ExternalLoginError
            {
                get
                {
                    var res = Converter.GetBool(HttpContextFactory.Current.Request.QueryString[QS.externalLoginError]);
                    return res.HasValue && res.Value;
                }
            }
        }
        public static string GetImgUrl(string url)
        {
            return string.Format("{0}/{1}", ImgFileServerUrl, url);
        }
        public static void ClearRequestSensitiveData()
        {
            ServiceId = null;
            LocationId = null;
        }
        public static int? ServiceId
        {
            get { return GetIntFromSession(QS.service); }
            set { HttpContextFactory.Current.Session[QS.service] = value; }
        }
        public static int? LocationId
        {
            get { return GetIntFromSession(QS.location); }
            set { HttpContextFactory.Current.Session[QS.location] = value; }
        }
        public static ApeekPrincipal AuthenticatedUser
        {
            get { return HttpContextFactory.Current.User as ApeekPrincipal; }
        }
        public static bool IsUserAuthenticated
        {
            get { return AuthenticatedUser != null && AuthenticatedUser.Identity.IsAuthenticated; }
        }
        public static List<string> IndexLocations { get; set; }
        public static void SetRequest(HttpRequestBase request)
        {
            _request = request;
        }
        public static int? GetIntFromSession(string qs)
        {
            var val = HttpContextFactory.Current.Session[qs];
            if (val != null)
                return Converter.GetInt(HttpContextFactory.Current.Session[qs].ToString());
            return null;
        }
        public static string GetErrorModel(string message)
        {
            if (HttpContextFactory.Current != null)
            {
                try
                {
                    var errorModel = new ErrorLogUtility(HttpContextFactory.Current.Request, new ApeekPrincipalSerializeModel(AuthenticatedUser)).GetErrorModel(message, HttpContextFactory.Current.Session);
                    return errorModel.ToString();
                }
                catch {}
            }
            return message;
        }
        public static LogEntry LogEntry(LogSource source, string message)
        {
            return LogEntry(source.ToString(), message);
        }
        public static LogEntry LogEntry(string source, string message)
        {
            var logEntry = new LogEntry();
            logEntry.ApplicationName = AppSettings.GetInstance().ApplicationName;
            logEntry.Message = message;
            logEntry.SourceName = source;
            logEntry.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (HttpContextFactory.Current != null)
            {
                if (HttpContextFactory.Current.Session != null)
                {
                    logEntry.SessionId = HttpContextFactory.Current.Session.SessionID;
                    if (HttpContextFactory.Current.Request != null)
                        logEntry.IpAddress = HttpContextFactory.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (AuthenticatedUser != null)
                    logEntry.UserId = AuthenticatedUser.UserId;
            }
            return logEntry;
        }
    }
}