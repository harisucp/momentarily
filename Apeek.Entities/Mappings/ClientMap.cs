using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class ClientMap : ClassMap<Client>
    {
        public ClientMap()
        {
            Table("c_client");
            Id(x => x.Id, "client_id");
            Map(x => x.Email, "email");
        }
    }
}