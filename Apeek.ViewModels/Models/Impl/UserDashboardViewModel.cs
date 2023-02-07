using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.ViewModels.Models.Impl
{
   public class UserDashboardViewModel
    {
       public int mostloanedItemsCount { get; set; }
       public List<MostRentedItems> mostloanedItems { get; set; }

        public int totalBorrowersCountbyUser { get; set; }
        public List<MostRentedItems> totalBorrowersListbyUser { get; set; }

        public double totalUserEarning { get; set; }
        public List<MostRentedItems> totalUserEarningListbyUser { get; set; }

        public double totalUserSpend { get; set; }
        public List<MostRentedItems> totalUserSpendListbyUser { get; set; }
        public List<MostRentedItems> totalUserEarningByMonth { get; set; }

        public List<MostRentedItems> totalUserSpendByMonth { get; set; }

    }
}
