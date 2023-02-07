using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class Audit : AuditEntity
    {
        public override int Id { get; set; }
        public override string TableName
        {
            get { return "s_audit"; }
        }
        public virtual int AuditTable { get; set; }
        public virtual int AuditAction { get; set; }
        public virtual int PrimaryKeyId { get; set; }
        public virtual string OldValue { get; set; }
        public virtual string NewValue { get; set; }
        public virtual string AffectedUsers { get; set; }
    }
}