using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class CountryMap : ClassMap<Country>
    {
        public CountryMap()
        {
            Table("d_country");
            Id(x => x.Id, "id");
            Map(x => x.Url, "url");
        }
    }
}