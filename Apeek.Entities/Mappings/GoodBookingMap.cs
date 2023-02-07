using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class GoodBookingMap : AuditEntityMap<GoodBooking>
    {
        public GoodBookingMap()
        {
            Table("c_good_booking");
            Id(x => x.GoodRequestId, "good_request_id").GeneratedBy.Assigned();
            Map(x => x.StartDate, "start_date");
            Map(x => x.EndDate, "end_date");
            Map(x => x.ShippingAddress, "shipping_address");
            Map(x => x.ApplyForDelivery, "apply_for_delivery");
            Map(x => x.StartTime, "start_time");
            Map(x => x.EndTime, "end_time");
            HasOne(x => x.GoodRequest);
        }
    }
}
