using System;
using System.Xml.Serialization;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Web.Sitemap
{
    public class SiteMapEntry
    {
        [XmlIgnore]
        public LocationLang LocationLang { get; set; }
        [XmlElement("loc")]
        public string Url { get; set; }
        [XmlElement("changefreq")]
        public ChangeFrequency ChangeFrequency { get; set; }
        [XmlElement("priority")]
        public string UpdatePriority { get; set; }
        public SiteMapEntry(LocationLang locationLang, string url, ChangeFrequency changeFrequency, string updatePriority)
        {
            LocationLang = locationLang;
            Url = url;
            ChangeFrequency = changeFrequency;
            UpdatePriority = updatePriority;
        }
        public SiteMapEntry() { }
    }
    public class SiteMapIndexEntry
    {
        [XmlElement("loc")]
        public string Url { get; set; }
        [XmlElement("lastmod")]
        public string ModDate { get; set; }
        public SiteMapIndexEntry(string url)
        {
            Url = url;
            ModDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss+02:00");
        }
        public SiteMapIndexEntry() { }
    }
}