using FluentNHibernate.Mapping;
namespace Apeek.Entities.Interfaces
{
    public abstract class AuditEntityMap<T> : ClassMap<T> where T : AuditEntity
    {
        public AuditEntityMap()
        {
            Map(x => x.CreateDate, "create_date");
            Map(x => x.CreateBy, "create_by");
            Map(x => x.ModDate, "mod_date");
            Map(x => x.ModBy, "mod_by");
        }
    }
    public abstract class AuditEntityUtMap<T> : ClassMap<T> where T : AuditEntityUt
    {
        public AuditEntityUtMap()
        {
            Map(x => x.CreateDateUt, "create_date");
            Map(x => x.CreateBy, "create_by");
            Map(x => x.ModDateUt, "mod_date");
            Map(x => x.ModBy, "mod_by");
        }
    }
}