using Apeek.Entities.Entities.Enums;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
    public class TwilioMessage : AuditEntity
    {
        public static string _TableName => "c_twilio_message";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual TwilioMessageType TwilioMessagesText { get; set; }
        public virtual bool Status { get; set; }
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
