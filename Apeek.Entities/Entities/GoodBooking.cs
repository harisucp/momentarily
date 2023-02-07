using System;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodBooking : AuditEntity
    {
        public static string _TableName = "c_good_booking";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int GoodRequestId { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual string ShippingAddress { get; set; }
        public virtual bool ApplyForDelivery { get; set; }
        public virtual GoodRequest GoodRequest { get; set; }
        public virtual string StartTime { get; set; }        public virtual string EndTime { get; set; }
    }
}
