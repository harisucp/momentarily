using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class CAMasterBulkEmailMap : AuditEntityMap<CAMasterBulkEmail>
    {
        public CAMasterBulkEmailMap()
        {
            Table("c_ca_master_bulk");
            Id(x => x.Id, "id");
            Map(x => x.EmailID, "email_id");
            Map(x => x.AddedToSendy, "added_to_sendy");
            
        }       
    }
}
 