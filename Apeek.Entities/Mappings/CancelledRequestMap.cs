using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class CancelledRequestMap : AuditEntityMap<CancelledRequest>
    {
        public CancelledRequestMap()
        {
            Table("c_cancel_request");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.RequestId, "request_id");
        }
    }
}
