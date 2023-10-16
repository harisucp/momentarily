using Apeek.Entities.Entities;
using Apeek.Entities.Entities.Enums;
using Apeek.Entities.Interfaces;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class TwilioMessageMap : AuditEntityMap<TwilioMessage>
    {
        public TwilioMessageMap()
        {
            Table("dbo.c_twilio_message");
            Id(x => x.Id);
            Map(x => x.TwilioMessagesText).CustomType<TwilioMessageType>();
            Map(x => x.Status);
            Map(x => x.UserId);
            References(x => x.User)
                .Column("id")
                .Not.Update()
                .Not.Insert();
        }
    }
}
