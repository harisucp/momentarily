using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class GoodRequestMap : AuditEntityMap<GoodRequest>
    {
        public GoodRequestMap()
        {
            Table("c_good_request");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.GoodId, "good_id");
            Map(x => x.StatusId, "status_id");
            Map(x => x.Price, "price");
            Map(x => x.Days, "days");
            Map(x => x.DaysCost, "days_cost");
            Map(x => x.CustomerCost, "customer_cost");
            Map(x => x.CustomerServiceFee, "customer_service_fee");
            Map(x => x.CustomerServiceFeeCost, "customer_service_fee_cost");
            Map(x => x.CustomerCharity, "customer_charity");
            Map(x => x.CustomerCharityCost, "customer_charity_cost");
            Map(x => x.SharerCost, "sharer_cost");
            Map(x => x.SharerServiceFee, "sharer_service_fee");
            Map(x => x.SharerServiceFeeCost, "sharer_service_fee_cost");
            Map(x => x.SharerCharity, "sharer_charity");
            Map(x => x.SharerCharityCost, "sharer_charity_cost");            
            Map(x => x.DiliveryCost, "dilivery_cost");
            Map(x => x.ShippingDistance, "shipping_distance");
            Map(x => x.DiliveryPrice, "dilivery_price");
            Map(x => x.SecurityDeposit, "security_deposit");
            Map(x => x.DiscountAmount, "discount_amount");
            Map(x => x.CouponCode, "coupon_code");
            Map(x => x.PendingAmount, "pending_amount");
            Map(x => x.IsUsedCoupon, "is_used_coupon");
            Map(x => x.IsViewed, "is_viewed");

            References(x => x.User)
                .Column("user_id")
                .Not.LazyLoad()
                .Not.Update()
                .Not.Insert();
            References(x => x.Good)
                .Column("good_id")
                .Not.LazyLoad()
                .Not.Update()
                .Not.Insert();
            References(x => x.RequestStatus)
                .Column("status_id")
                .Not.Update()
                .Not.Insert();
            HasOne(x => x.GoodBooking);
        }
    }
}
