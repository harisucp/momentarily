using Apeek.Core.Interfaces;
using Apeek.Common;
using Apeek.Core.Services;
namespace Apeek.Core.Web
{
    public class WebViewController
    {
        private ITranslateController _translate;
        public IUrlGenerator UrlGenerator { get; set; }
        private ISettingsDataService _settingsDataService;
        public WebViewController()
        {
            _translate = Ioc.Get<ITranslateController>();
            UrlGenerator = Ioc.Get<IUrlGenerator>();
            _settingsDataService = Ioc.Get<ISettingsDataService>();
        }
        /// <summary>
        /// Translates given word to current language
        /// </summary>
        public string t(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            return _translate[str];
        }
        /// <summary>
        /// Translate given word to current language and transform according to given case
        /// </summary>
        public string tc(string str, Cases c)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            return _translate.TranslateCase(str, c);
        }
        public bool IsProduction
        {
            get
            {
                return _settingsDataService.GetIsProduction();
            }
        }
        public string GetUrl(int id, string url)
        {
            return UrlGenerator.GetUrl(id, url);
        }
    }
}