using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class UserImg : IUserImg, IEntity
    {
        public static string _TableName = "c_user_img";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual int Type { get; set; }
        public virtual int Sequence { get; set; }
        public virtual string FileName { get; set; }
    }
}