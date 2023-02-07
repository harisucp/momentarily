using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
    public class SendyListMap : AuditEntityMap<SendyList>
    {
        public SendyListMap()
        {
            Table("c_sendy_list");
            Id(x => x.Id, "id");
            Map(x => x.ListID, "list_id");
            Map(x => x.IsCampaignSend, "is_campaign_send");
            Map(x => x.ListName, "list_name");
        }
    }
}
