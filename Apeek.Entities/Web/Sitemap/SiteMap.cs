using System.Collections.Generic;
using System.Xml.Serialization;
namespace Apeek.Entities.Web.Sitemap
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SiteMap
    {
        [XmlElement("url")]
        public List<SiteMapEntry> Urls { get; set; }
        public SiteMap()
        {
            Urls = new List<SiteMapEntry>();
        }
    }
    [XmlRoot("sitemapindex", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SiteMapIndex
    {
        [XmlElement("sitemap")]
        public List<SiteMapIndexEntry> IndexEntry { get; set; }
        public SiteMapIndex()
        {
            IndexEntry = new List<SiteMapIndexEntry>();
        }
    }
}