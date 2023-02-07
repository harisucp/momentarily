using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class ServiceLangMap : ClassMap<ServiceLang>
    {
        public ServiceLangMap()
        {
            Table("c_service_lang");
            Id(x => x.Id, "id");
            Map(x => x.Lang_Id, "lang_id");
            Map(x => x.Name, "name");
            Map(x => x.Url, "url");
            Map(x => x.Path, "path");
            References(x => x.Service, "service_id").Fetch.Join();
        }
    }
}