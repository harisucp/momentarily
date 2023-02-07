using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momentarily.ViewModels.Models
{
   public class FAQVM
    {
        public  string Filter { get; set; }
        public List<FAQList> FAQLists { get; set; }

    }
    public class FAQList
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Type { get; set; }
    }


}
