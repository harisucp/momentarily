using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class GlobalCodeMap : AuditEntityMap<GlobalCodes>
    {
        public GlobalCodeMap()
        {
            Table("c_global_code");
            Id(x => x.Id, "id");
            Map(x => x.GlobalCodeName, "global_code_name");
            Map(x => x.GlobalCodeCategoryId, "global_code_categoryId");
            
         
        }
    }
}
