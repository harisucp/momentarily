using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class TwilioLogMessageMap : AuditEntityMap<TwilioLogMessage>
    {
        public TwilioLogMessageMap()
        {
            Table("dbo.c_twilio_log_message");
            Id(x => x.Id);
            Map(x => x.Message);
            Map(x => x.UserId);
            References(x => x.User)
                .Column("id")
                .Not.Update()
                .Not.Insert();
        }
    }
}
