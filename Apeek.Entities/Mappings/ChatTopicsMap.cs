using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
  public class ChatTopicsMap : AuditEntityMap<ChatTopics>
    {
        public ChatTopicsMap()
        {
            Table("c_cb_topics");
            Id(x => x.Id, "id");
            Map(x => x.Name, "name");
        }
    }
}
