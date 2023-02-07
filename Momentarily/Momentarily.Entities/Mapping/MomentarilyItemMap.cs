using Apeek.Entities.Interfaces;
using Momentarily.Entities.Entities;
namespace Momentarily.Entities.Mapping
{
    public class MomentarilyItemMap : AuditEntityMap<MomentarilyItem>
    {
        public MomentarilyItemMap()
        {
            Table("c_good");
            Id(x => x.Id, "id");
            Map(x => x.Name, "name");
            Map(x => x.Description, "description");
            Map(x => x.MinimumRentPeriod, "minimum_rent_period");
            Map(x => x.Price, "price");
            Map(x => x.IsArchive, "is_archive");
            Map(x => x.PricePerWeek, "price_per_week");
            Map(x => x.PricePerMonth, "price_per_month");
            Map(x => x.RentPeriodDay, "rent_period_day");
            Map(x => x.RentPeriodWeek, "rent_period_week");
            Map(x => x.RentPeriodMonth, "rent_period_month");
            Map(x => x.AgreeToDeliver, "agree_to_deliver");
            Map(x => x.AgreeToShareImmediately, "agree_to_share_immediately");
            HasOne(x => x.GoodLocation);
            HasOne(x => x.StartEndDate);
            HasMany(x => x.GoodImages)
                .KeyColumn("good_id")
                .Inverse()
                .Fetch.Join();
        }
    }
}