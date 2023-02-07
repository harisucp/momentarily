using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
    
    public class Subscribes : AuditEntity
    {

        public static string _TableName = "c_subscribe_email";
       
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        public virtual string Email { get; set; }
        public virtual bool SubscribeStatus { get; set; }
    }
}
