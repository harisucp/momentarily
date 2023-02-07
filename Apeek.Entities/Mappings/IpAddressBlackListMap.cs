using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class IpAddressBlackListMap : ClassMap<IpAddressBlackList>
    {
        public IpAddressBlackListMap()
        {
            Table("s_ip_address_black_list");
            Id(x => x.Id, "id");
            Map(x => x.Ip, "ip");
        }
    }
}