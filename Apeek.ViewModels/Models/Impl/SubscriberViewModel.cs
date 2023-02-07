using Apeek.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.ViewModels.Models.Impl
{
   public class SubscriberViewModel
    {
        public string Status { get; set; }
        public List<SubscriberVM> subscribers { get; set; }

        public class SubscriberVM
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public bool SubscribeStatus { get; set; }
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime? CreatedDate { get; set; }

        }
    }
}
