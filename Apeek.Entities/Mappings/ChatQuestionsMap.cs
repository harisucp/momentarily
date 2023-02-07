using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class ChatQuestionsMap : AuditEntityMap<ChatQuestions>
    {
        public ChatQuestionsMap()
        {
            Table("c_cb_questions");
            Id(x => x.Id, "id");
            Map(x => x.Question, "question");
            Map(x => x.TopicId, "topic_id");
            Map(x => x.AnsId, "ans_id");
        }
    }
}
