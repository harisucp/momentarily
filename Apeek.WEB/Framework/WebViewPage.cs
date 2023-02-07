using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Common.Extensions;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Web;
using Apeek.Entities.Web;

namespace Apeek.Web.Framework
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private WebViewController _controller;

        public WebViewPage()
        {
            _controller = new WebViewController();
        }

        //calls before page is initialized
        protected override void InitializePage()
        {
            QuickUrl = new QuickUrl(new MvcUrlHelper(Url), _controller.UrlGenerator);
            
            base.InitializePage();
        }

        public new ApeekPrincipal User
        {
            get
            {
                var user = base.User as ApeekPrincipal;
                if(user == null)
                    return Context.User as ApeekPrincipal;

                return user;
            }
        }

        public T ParseEnum<T>(string value)
        {
            return EnumHelper.ParseEnum<T>(value);
        }

        public bool IsProduction
        {
            get
            {
                return _controller.IsProduction;
            }
        }

        public virtual TModel Shape { get { return Model; } }
        public QuickUrl QuickUrl { get; private set; }

        /// <summary>
        /// Translates given word to current language
        /// </summary>
        public string t(string str)
        {
            return _controller.t(str);
        }

        /// <summary>
        /// Translate given word to current language and transform according to given case
        /// </summary>
        public string tc(string str, Cases c)
        {
            return _controller.tc(str, c);
        }

        public bool IsHome()
        {
            return (ViewContext.RouteData.Values["action"] == "Index") && (ViewContext.RouteData.Values["controller"] == "Home");
        }

        public string GetUrl(int id, string url)
        {
            return _controller.UrlGenerator.GetUrl(id, url);
        }

        public Language l
        {
            get { return LanguageController.CurLang; }
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {

    }
}