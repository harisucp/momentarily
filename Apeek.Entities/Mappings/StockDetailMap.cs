using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class StockDetailMap : AuditEntityMap<StockDetail>
    {
        public StockDetailMap()
        {
            Table("c_stock_detail");
            Id(x => x.Id, "id");
            Map(x => x.CovidGoodId, "covid_good_id");
            Map(x => x.Quantity, "quantity");
            Map(x => x.Description, "description");
           
        }
    }
}
