using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.ViewModels.Models.Impl
{
   public class CAMasterViewModel
    {
        public string Email { get; set; }
        public bool AddedToSendy { get; set; }
        public int AddedCount { get; set; }
        public bool ReturnStatus { get; set; }

        public List<CAMasterViewModel> cAMasterViewModelsList { get; set; }
    }
}
