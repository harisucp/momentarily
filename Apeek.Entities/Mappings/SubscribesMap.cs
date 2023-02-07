using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
  public class SubscribesMap : AuditEntityMap<Subscribes>
    {
        public SubscribesMap()
        {
            Table("c_subscribe_email");
            Id(x => x.Id, "id");
            Map(x => x.Email, "email_id");
            Map(x => x.SubscribeStatus, "subscribe_status");
        }

}
}
