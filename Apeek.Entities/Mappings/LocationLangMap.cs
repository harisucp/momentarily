using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class LocationLangMap : ClassMap<LocationLang>
    {
        public LocationLangMap()
        {
            Table("d_location_lang");
            Id(x => x.Id, "location_lang_id");
            Map(x => x.Lang_Id, "lang_id");
            Map(x => x.Name, "name");
            Map(x => x.Url, "url");
            Map(x => x.SubDomainUrl, "sub_domain_url");
            References(x => x.Location, "location_id");
        }
    }
    public class LocationLangDescrMap : ClassMap<LocationLangDescr>
    {
        public LocationLangDescrMap()
        {
            Table("d_location_lang");
            Id(x => x.Id, "location_lang_id");
            Map(x => x.Name, "name");
            Map(x => x.Lang_Id, "lang_id");
            Map(x => x.LocationId, "location_id");
            Map(x => x.Description, "description");
            Map(x => x.ShortDescription, "short_description");
        }
    }
}