using System.Collections.Generic;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Entities;
using Apeek.Entities.Validators;
using Apeek.Entities.Web;
namespace Apeek.Common
{
    public class BreadcrumbsBuilder
    {
        private readonly QuickUrl _quickUrl;
        private readonly string _titleTemplate;
        private List<BreadcrumbsEntry> _breadcrumbsEntries;
        private List<BreadcrumbsEntry> _breadcrumbsEntryLocations;
        public BreadcrumbsBuilder(QuickUrl quickUrl, string titleTemplate)
        {
            _quickUrl = quickUrl;
            _titleTemplate = titleTemplate;
            Initialize();
        }
        public List<BreadcrumbsEntry> GetBreadcrumbsEntryLocations()
        {
            return _breadcrumbsEntryLocations;
        }
        public List<BreadcrumbsEntry> GetBreadcrumbsEntries()
        {
            return _breadcrumbsEntries;
        }
        private void Initialize()
        {
            _breadcrumbsEntryLocations = new List<BreadcrumbsEntry>();
            _breadcrumbsEntries = new List<BreadcrumbsEntry>();
        }
        public void AddBreadcrumbsEntry(string title, string url)
        {
            _breadcrumbsEntries.Add(BuildBreadcrumbsEntry(title, url));
        }
        public void BuildBreadcrumbsEntry(List<LocationLang> locationLangs, List<ServiceLang> serviceLangs, string currentServiceUrl)
        {
            foreach (var locationLang in locationLangs)
            {
                var be = new BreadcrumbsEntry();
                string url;
                be.Title = string.Format(_titleTemplate, locationLang.Name);
                if (!string.IsNullOrWhiteSpace(currentServiceUrl))
                    url = _quickUrl.BrowseUrl(locationLang, currentServiceUrl);
                else
                    url = _quickUrl.LocationUrl(locationLang);
                be.Url = url;
                _breadcrumbsEntryLocations.Add(be);
            }
            foreach (var serviceLang in serviceLangs)
            {
                if(serviceLang.Service.Status != ServiceStatus.Verified)
                    continue;
                var be = new BreadcrumbsEntry();
                be.Title = string.Format(_titleTemplate, serviceLang.Name.ToUpperFirstChar());
                be.Url = _quickUrl.BrowseUrl(serviceLang);
                _breadcrumbsEntries.Add(be);
            }
        }
        private BreadcrumbsEntry BuildBreadcrumbsEntry(string title, string url)
        {
            return new BreadcrumbsEntry()
            {
                Title = title,
                Url = url
            };
        }
    }
}