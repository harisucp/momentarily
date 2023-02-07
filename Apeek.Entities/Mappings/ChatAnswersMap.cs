using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
  public class ChatAnswersMap : AuditEntityMap<ChatAnswers>
    {
        public ChatAnswersMap()
        {
            Table("c_cb_answers");
            Id(x => x.Id, "id");
            Map(x => x.Answer, "answer");
            Map(x => x.AnswerType, "answer_type");
            Map(x => x.QuesId, "ques_id");
        }
    }
}
