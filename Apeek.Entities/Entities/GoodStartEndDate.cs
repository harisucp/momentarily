using System;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodStartEndDate : AuditEntity
    {
        public static string _TableName = "c_good_start_end_date";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int GoodId { get; set; }
        public virtual DateTime? DateStart { get; set; }
        public virtual DateTime? DateEnd { get; set; }
    }
}
