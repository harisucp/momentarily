using System;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodBookingRank : AuditEntity
    {
        public static string _TableName = "c_good_booking_rank";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int GoodRequestId { get; set; }
        public virtual int GoodId { get; set; }
        public virtual int SharerId { get;set; }
        public virtual int SeekerId { get; set; }
        public virtual int ReviewerId { get; set; }
        public virtual int Rank { get; set; }
        public virtual string Message { get; set; }
        public virtual User Reviewer { get; set; }
        public virtual bool is_deleted { get; set; }
        
    }
}
