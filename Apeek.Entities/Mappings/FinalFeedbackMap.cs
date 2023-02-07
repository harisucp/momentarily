using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class FinalFeedbackMap : AuditEntityMap<FinalFeedback>
    {
        public FinalFeedbackMap()
        {
            Table("c_final_feedback");
            Id(x => x.Id, "id");
            Map(x => x.RequestId, "request_id");
            Map(x => x.NoIssue, "no_issue");
            Map(x => x.Late, "late");
            Map(x => x.Damaged, "damaged");
            Map(x => x.ReturnDate, "returned_date");
            Map(x => x.ReturnTime, "return_time");
            Map(x => x.Claim, "claim");
            Map(x => x.Description, "description");
        }
    }
}
