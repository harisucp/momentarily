using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class DnsMap : ClassMap<Dns>
    {
        public DnsMap()
        {
            Table("s_dns");
            Id(x => x.DnsId, "dns_id");
            Map(x => x.Name, "dns");
            Map(x => x.DefaultLangId, "default_lang_id");
            Map(x => x.IsDefault, "is_default");
        }
    }
}