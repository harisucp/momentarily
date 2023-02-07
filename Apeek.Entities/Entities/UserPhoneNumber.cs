using Apeek.Entities.Interfaces;

namespace Apeek.Entities.Entities
{
    public class UserPhoneNumber : AuditEntity
    {
        public static string _TableName = "c_user_phone_number";

        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }

        public virtual int UserId { get; set; }
        public virtual string PhoneNumber { get; set; }
    }
}