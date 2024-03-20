using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class LiveLocationMap : AuditEntityMap<LiveLocation>
    {
        public LiveLocationMap()
        {
            Table("dbo.d_live_location");
            Id(x => x.Id);
            Map(x => x.SharerLatitude);
            Map(x => x.SharerLongitude);
            Map(x => x.BorrowerLatitude);
            Map(x => x.BorrowerLongitude);
            Map(x => x.LocationId);
            Map(x => x.RideStarted);
            Map(x => x.DeliveryConfirm);
            Map(x => x.ReturnConfirm);
            Map(x => x.DeliverBy).CustomType<DeliverBy>();
            Map(x => x.SharerId);
            References(x => x.SharerUser)
                .Column("id")
                .Not.Update()
                .Not.Insert();
            Map(x => x.BorrowerId);
            References(x => x.BorrowerUser)
                .Column("id")
                .Not.Update()
                .Not.Insert();
            Map(x => x.GoodRequestId);
            References(x => x.GoodRequest)
                .Column("Id")
                .Not.Update()
                .Not.Insert();
        }
    }
}