namespace Apeek.Entities.Entities
{
    public class UserPrivilege
    {
        public static string _TableName = "s_user_privilege";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual string PrivilegeName { get; set; }
        public virtual bool HasAccess { get; set; }
    }
}