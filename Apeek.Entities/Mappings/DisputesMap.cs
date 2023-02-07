using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class DisputesMap : AuditEntityMap<Disputes>
    {
        public DisputesMap()
        {
            Table("c_disputes");
            Id(x => x.Id, "id");
            Map(x => x.RequestId, "request_id");
            Map(x => x.DisputeCreatedBy, "dispute_created_by");
            Map(x => x.LastStatus, "last_status");
            Map(x => x.Reason, "reason");
            Map(x => x.Description, "description");
            
        }
    }
}
