using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class GoodMap : AuditEntityMap<Good>
    {
        public GoodMap()
        {
            Table("c_good");
            Id(x => x.Id, "id");
            Map(x => x.Name, "name");
            Map(x => x.Description, "description");
            Map(x => x.MinimumRentPeriod, "minimum_rent_period");
            Map(x => x.Price, "price");
            Map(x => x.PricePerWeek, "price_per_week"); 
            Map(x => x.IsArchive, "is_archive");
            Map(x => x.IsViewed, "is_viewed");
            HasOne(x => x.GoodLocation);
            HasMany(x => x.GoodImages)
                .KeyColumn("good_id")
                .Inverse()
                .Fetch.Join();
        }   
    }
}