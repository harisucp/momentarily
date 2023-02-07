using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class CovidGoodMap : AuditEntityMap<CovidGood>
    {
        public CovidGoodMap()
        {
            Table("c_covid_good");
            Id(x => x.Id, "id");
            Map(x => x.Name, "name");
            Map(x => x.Description, "description");
            Map(x => x.Price, "price");
            Map(x => x.Image, "image");
       }
    }
}
