using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
    public class LiveLocation : AuditEntity
    {
        public static string _TableName => "d_live_location";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual string LocationId { get; set; }
        public virtual double? SharerLatitude { get; set; }
        public virtual double? SharerLongitude { get; set; }
        public virtual double? BorrowerLatitude { get; set; }
        public virtual double? BorrowerLongitude { get; set; }
        public virtual int? SharerId { get; set; }
        public virtual User SharerUser { get; set; }
        public virtual int? BorrowerId { get; set; }
        public virtual User BorrowerUser { get; set; }
        public virtual int? GoodRequestId { get; set; }
        public virtual GoodRequest GoodRequest { get; set; }
        public virtual DeliverBy? DeliverBy { get; set; }
        public virtual bool? DeliveryConfirm { get; set; }
        public virtual bool? ReturnConfirm { get; set; }
        public virtual bool? RideStarted { get; set; }
    }
    public enum DeliverBy
    {
        BORROWER,
        SHARER
    }
}
