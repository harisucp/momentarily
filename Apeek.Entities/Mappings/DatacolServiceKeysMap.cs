using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class DatacolServiceKeysMap : ClassMap<DatacolServiceKeys>
    {
        public DatacolServiceKeysMap()
        {
            Table("s_datacol_service_keys");
            Id(x => x.Id, "id");
            Map(x => x.ServiceId, "service_id");
            Map(x => x.Keys, "keys");
        }     
    }
}