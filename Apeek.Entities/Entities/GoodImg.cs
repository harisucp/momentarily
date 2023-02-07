using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodImg : IUserImg, IEntity
    {
        public static string _TableName = "c_good_img";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual int? GoodId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int Type { get; set; }
        public virtual int Sequence { get; set; }
        public virtual string FileName { get; set; }
        public virtual string Folder { get; set; }
    }
}