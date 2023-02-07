using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.ViewModels.Models.Impl
{
   public class ChatBotQuestionsŌrAnswersVM
    {
        public int QuesId { get; set; }
        public string Question { get; set; }
        public int TopicId { get; set; }
        public int AnsId { get; set; }
        public string Answer { get; set; }
        public bool AnswerType { get; set; }
    }
}
