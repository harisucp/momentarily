using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Apeek.Common;
using Apeek.Common.Configuration;
using Apeek.Common.Controllers;
using Apeek.Common.Converters.TypeConverters;
using Apeek.Common.UrlHelpers;
using Apeek.Entities.Web.Sitemap;
namespace Apeek.Core.Services
{
    public interface ISiteMapDataProvider
    {
        List<ISiteMapDataProvider> SiteMapDataProviders { get; set; }
        string BuildSiteMap(SiteMapEntry siteMapEntry);
    }
    public abstract class SiteMapDataProviderBase : ISiteMapDataProvider
    {
        public abstract List<ISiteMapDataProvider> SiteMapDataProviders { get; set; }
        public abstract string BuildSiteMap(SiteMapEntry siteMapEntry);
        protected QuickUrl QuickUrl { get; set; }
        public const string SiteMapName = "sitemap.xml";
        public const string SiteMapSubDomainName = "sitemap-{0}.xml";
        public const string SiteMapSubDomainCatName = "sitemap-{0}-cat.xml";
        public const string SiteMapSubDomainUserName = "sitemap-{0}-users.xml";
        protected SiteMapDataProviderBase(IUrlHelper url)
        {
            QuickUrl = new QuickUrl((MvcUrlHelper) url, new UrlGenerator());
        }
        protected void SaveToFile<T>(T obj, string fileName)
        {
            var xml = XmlConverter<T>.ObjectToXml(obj);
            if (!Directory.Exists(AppSettings.GetInstance().SiteMapDirectory))
                Directory.CreateDirectory(AppSettings.GetInstance().SiteMapDirectory);
            File.WriteAllText(string.Format(@"{0}\{1}", AppSettings.GetInstance().SiteMapDirectory, fileName), xml, Encoding.UTF8);
        }
    }
    public class SiteMapDataProvider : SiteMapDataProviderBase
    {
        public override List<ISiteMapDataProvider> SiteMapDataProviders { get; set; }
        public SiteMapDataProvider(IUrlHelper url) : base(url)
        {
            SiteMapDataProviders = new List<ISiteMapDataProvider>();
            SiteMapDataProviders.Add(new SiteMapLocationDataProvider(QuickUrl.UrlHelper));
        }
        public override string BuildSiteMap(SiteMapEntry siteMapEntry)
        {
            var siteMapIndex = new SiteMapIndex();
            var subDomainsEntries = new List<SiteMapEntry>(); //new SitemapDataService(QuickUrl).GetLocationForIndexSitemapEntry();
            foreach (var siteMapDataProvider in SiteMapDataProviders)
            {
                foreach (var subDomainsEntry in subDomainsEntries)
                {
                    string fileName = siteMapDataProvider.BuildSiteMap(subDomainsEntry);
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        var url = string.Format("{0}://{1}.{2}/{3}",
                            QuickUrl.UrlHelper.RequestContext.HttpContext.Request.Url.Scheme,
                            subDomainsEntry.LocationLang.SubDomainUrl,
                            ApeekController.GetCurrentDnsWithPort(QuickUrl.UrlHelper.RequestContext.HttpContext.Request.Url),
                            fileName);
                        siteMapIndex.IndexEntry.Add(new SiteMapIndexEntry(url));
                    }
                }
            }
            SaveToFile(siteMapIndex, SiteMapName);
            return SiteMapName;
        }
    }
    public class SiteMapLocationDataProvider : SiteMapDataProviderBase
    {
        public string FileName { get; set; }
        public override List<ISiteMapDataProvider> SiteMapDataProviders { get; set; }
        public SiteMapLocationDataProvider(IUrlHelper url) : base(url)
        {
            SiteMapDataProviders = new List<ISiteMapDataProvider>();
            SiteMapDataProviders.Add(new SiteMapCatDataProvider(QuickUrl.UrlHelper));
            SiteMapDataProviders.Add(new SiteMapUsersDataProvider(QuickUrl.UrlHelper));
            //SiteMapDataProviders.Add(new SiteMapServicesDataProvider());
        }
        public override string BuildSiteMap(SiteMapEntry siteMapEntry)
        {
            var siteMapIndex = new SiteMapIndex();
            foreach (var siteMapDataProvider in SiteMapDataProviders)
            {
                var fileName = siteMapDataProvider.BuildSiteMap(siteMapEntry);
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    var url = string.Format("{0}://{1}.{2}/{3}",
                        QuickUrl.UrlHelper.RequestContext.HttpContext.Request.Url.Scheme,
                        siteMapEntry.LocationLang.SubDomainUrl,
                        ApeekController.GetCurrentDnsWithPort(QuickUrl.UrlHelper.RequestContext.HttpContext.Request.Url),
                        fileName);
                    siteMapIndex.IndexEntry.Add(new SiteMapIndexEntry(url));
                }
            }
            string xmlFileName = string.Format(SiteMapSubDomainName, siteMapEntry.LocationLang.SubDomainUrl);
            SaveToFile(siteMapIndex, xmlFileName);
            return xmlFileName;
        }
    }
    public class SiteMapCatDataProvider : SiteMapDataProviderBase
    {
        public override List<ISiteMapDataProvider> SiteMapDataProviders { get; set; }
        public SiteMapCatDataProvider(IUrlHelper url) : base(url)
        {
            SiteMapDataProviders = new List<ISiteMapDataProvider>();
        }
        public override string BuildSiteMap(SiteMapEntry siteMapEntry)
        {
            var siteMap = new SiteMap();
            var sitemapEntries = new List<SiteMapEntry>(); //new SitemapDataService(QuickUrl).GetBrowsSitemapEntry(siteMapEntry.LocationLang.Location.Id);
            foreach (var sitemapEntry in sitemapEntries)
            {
                siteMap.Urls.Add(sitemapEntry);
            }
            string xmlFileName = string.Format(SiteMapSubDomainCatName, siteMapEntry.LocationLang.SubDomainUrl);
            SaveToFile(siteMap, xmlFileName);
            return xmlFileName;
        }
    }
    public class SiteMapUsersDataProvider : SiteMapDataProviderBase
    {
        public override List<ISiteMapDataProvider> SiteMapDataProviders { get; set; }
        public SiteMapUsersDataProvider(IUrlHelper url): base(url)
        {
            SiteMapDataProviders = new List<ISiteMapDataProvider>();
        }
        public override string BuildSiteMap(SiteMapEntry siteMapEntry)
        {
            var siteMap = new SiteMap();
            var sitemapEntries = new List<SiteMapEntry>();//new SitemapDataService(QuickUrl).GetUsersSitemapEntry(siteMapEntry.LocationLang);
            foreach (var sitemapEntry in sitemapEntries)
            {
                siteMap.Urls.Add(sitemapEntry);
            }
            string xmlFileName = string.Format(SiteMapSubDomainUserName, siteMapEntry.LocationLang.SubDomainUrl);
            SaveToFile(siteMap, xmlFileName);
            return xmlFileName;
        }
    }
    public class SiteMapServicesDataProvider : SiteMapDataProviderBase
    {
        public override List<ISiteMapDataProvider> SiteMapDataProviders { get; set; }
        public SiteMapServicesDataProvider(IUrlHelper url) : base(url)
        {
            SiteMapDataProviders = new List<ISiteMapDataProvider>();
        }
        public override string BuildSiteMap(SiteMapEntry siteMapEntry)
        {
            throw new System.NotImplementedException();
        }
    }
}