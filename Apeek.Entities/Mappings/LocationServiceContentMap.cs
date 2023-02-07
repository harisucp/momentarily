using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class LocationServiceContentMap : ClassMap<LocationServiceContent>
    {
        public LocationServiceContentMap()
        {
            Table("c_location_service_content");
            CompositeId()
                .KeyProperty(x => x.ServiceId, "service_id")
                .KeyProperty(x => x.LocationId, "location_id")
                .KeyProperty(x => x.LangId, "lang_id");
            Map(x => x.Description, "description");
            Map(x => x.MetaDescr, "meta_descr");
            Map(x => x.MetaTitle, "meta_title");
            Map(x => x.MetaKeys, "meta_keys");
            Map(x => x.HeaderText, "header_text");
        }
    }
}