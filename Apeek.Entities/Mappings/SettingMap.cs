using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class SettingMap : ClassMap<Setting>
    {
        public SettingMap()
        {
            Table("s_settings");
            Id(x => x.Id, "id");
            Map(x => x.Key, "`key`");
            Map(x => x.Value, "`value`");
        }
    }
}