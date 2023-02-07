using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class RequestStatus : AuditEntity
    {
        public static string _TableName = "c_request_status";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual string StatusName { get; set; }
    }
}
