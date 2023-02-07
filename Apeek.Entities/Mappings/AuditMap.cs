using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class AuditMap : AuditEntityMap<Audit>
    {
        public AuditMap()
        {
            var tableName = "s_audit";
            Table(tableName);
            Id(x => x.Id, "audit_id");
            Map(x => x.AuditTable, "audit_table");
            Map(x => x.AuditAction, "audit_action");
            Map(x => x.PrimaryKeyId, "primary_key_id");
            Map(x => x.NewValue, "new_value");
            Map(x => x.OldValue, "old_value");
            Map(x => x.AffectedUsers, "affected_users");
        }
    }
}