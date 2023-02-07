using System.Web.Mvc;
using Apeek.Core.Web;
using Apeek.Common;
using Apeek.Common.HttpContextImpl;
using Apeek.Common.UrlHelpers;
namespace Apeek.Core.RazorRenderEngine
{
    public abstract class RazorTemplateBase<T> : TemplateBase
    {
        public T Model { get; set; }
        public QuickUrl QuickUrl { get; private set; }
        public UrlHelper Url { get; private set; }
        private WebViewController _controller;
        public string Output { get { return _output.ToString(); } }
        protected RazorTemplateBase()
        {
            _controller = new WebViewController();
            Url = new UrlHelper(HttpContextFactory.Current.Request.RequestContext);
            QuickUrl = new QuickUrl(new MvcUrlHelper(Url), _controller.UrlGenerator);
        }
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
        public string GetUrl(int id, string url)
        {
            return _controller.UrlGenerator.GetUrl(id, url);
        }
    }
}
