using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class CovidGoodOrderMap : AuditEntityMap<CovidGoodOrder>
    {
        public CovidGoodOrderMap()
        {
            Table("c_covid_good_order");
            Id(x => x.Id, "id");
            Map(x => x.CovidGoodId, "covid_good_id");
            Map(x => x.OrderPrice, "order_price");
            Map(x => x.Quantity, "quantity");
            Map(x => x.TotalPrice, "total_price");
            Map(x => x.BuyerEmailId, "buyer_email_id");
            Map(x => x.Description, "description");
            Map(x => x.StatusId, "status_id");
            Map(x => x.Tax, "tax");
            Map(x => x.DeliveryCharge, "delivery_charge");
            Map(x => x.DeliveryAddress1, "delivery_address1");
            Map(x => x.DeliveryAddress2, "delivery_address2");
            Map(x => x.City, "city");
            Map(x => x.State, "state");
            Map(x => x.Country, "country");
            Map(x => x.ZipCode, "zip_code");
            Map(x => x.FullName, "full_name");
            
        }
    }
}
