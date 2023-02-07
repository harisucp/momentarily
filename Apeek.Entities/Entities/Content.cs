using System;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class Content : IEntity
    {
        public static string _TableName = "c_content";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual bool Hidden { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual DateTime? ModDate { get; set; }
        public virtual int CreateBy { get; set; }
        public virtual int ModBy { get; set; }
        public virtual string PageName { get; set; }
    }
}