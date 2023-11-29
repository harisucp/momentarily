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
        public int ReviewCount { get; set; }
        public decimal Rank
        {
            get
            {
                if (SeekersCountReview == 0) return RankSharers;
                if (SharersCountReview == 0) return RankSeekers;
                return (RankSeekers + RankSharers) / 2;
            }
        }
        public int TotalRentals { get; set; }
        public int TotalCancelledRentals { get; set; }
        public UserPublicProfile userPublicProfile { get; set; }
        public int SharersCountReview { get; set; }
        public int SeekersCountReview { get; set; }
        public decimal RankSharers { get; set; }
        public decimal RankSeekers { get; set; }
        public ListViewModel<ReviewViewModel> SharersReviews { get; set; }
        public ListViewModel<ReviewViewModel> SeekersReviews { get; set; }
        public ListViewModel<UserProfileGoodViewModel> Listings { get; set; }

        public UserDashboardViewModel()
        {
            Listings = new ListViewModel<UserProfileGoodViewModel>();
            SeekersReviews = new ListViewModel<ReviewViewModel>();
            SharersReviews = new ListViewModel<ReviewViewModel>();
        }

    }
}
