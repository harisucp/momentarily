using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class CountryLangMap : ClassMap<CountryLang>
    {
        public CountryLangMap()
        {
            Table("d_countryLang");
            Id(x => x.Id, "id");
            Map(x => x.Name, "name");
            Map(x => x.Lang_Id, "lang_id");
            References(x => x.Country, "country_id");
        }
    }
}