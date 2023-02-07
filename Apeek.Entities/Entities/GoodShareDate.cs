using Apeek.Entities.Interfaces;
using System;
namespace Apeek.Entities.Entities
{
    public class GoodShareDate : IEntity
    {
        public static string _TableName = "c_good_share_date";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual int GoodId { get; set; }
        public virtual DateTime ShareDate { get; set; }
        public virtual string StartTime { get; set; }        public virtual string EndTime { get; set; }
    }
}
