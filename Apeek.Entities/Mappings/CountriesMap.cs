using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class CountriesMap : AuditEntityMap<Countries>
    {
        public CountriesMap()
        {
            Table("c_countries");
            Id(x => x.Id, "id");
            Map(x => x.ISO, "iso");
            Map(x => x.Name, "name");
            Map(x => x.NiceName, "nicename");
            Map(x => x.ISO3, "iso3");
            Map(x => x.NumCode, "numcode");
            Map(x => x.PhoneCode, "phonecode");
        }
    }
}
