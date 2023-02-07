using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class FAQMap : AuditEntityMap<FAQ>
    {
        public FAQMap()
        {
            Table("c_faq");
            Id(x => x.Id, "id");
            Map(x => x.Question, "faq_question");
            Map(x => x.Answer, "faq_answer");
            Map(x => x.Type, "faq_type");
        }
    }
}
