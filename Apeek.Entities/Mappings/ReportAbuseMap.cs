using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class ReportAbuseMap : AuditEntityMap<ReportAbuse>
    {
        public ReportAbuseMap()
        {
            Table("c_report_abuse");
            Id(x => x.Id, "id");
            Map(x => x.GoodId, "good_id");
            Map(x => x.UserId, "user_id");
            Map(x => x.Description, "description");
            Map(x => x.count, "count");
            Map(x => x.GlobalCodeId, "global_code_id");
        }
    }
}
