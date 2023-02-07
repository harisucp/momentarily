using Apeek.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.ViewModels.Models.Impl
{
   public class UserListing
    {
        public string Status { get; set; }
       public List<User> users { get; set; }
    }
}
