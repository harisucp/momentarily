using System.Collections.Generic;
namespace Apeek.ViewModels.Models.Impl
{
    public class UserPublicProfile
    {
        public UserPublicProfile()
        {
            SeekersReviews = new ListViewModel<ReviewViewModel>();
            SharersReviews = new ListViewModel<ReviewViewModel>();
            Listings = new ListViewModel<UserProfileGoodViewModel>();
        }
        public UserViewModel User { get; set; }
        public decimal Rank {
            get
            {
                if (SeekersCountReview == 0) return RankSharers;
                if (SharersCountReview == 0) return RankSeekers;
                return (RankSeekers + RankSharers)/2;
            }
        }
        public int ReviewCount {
            get { return SeekersCountReview + SharersCountReview; }
        }
        public decimal RankSharers { get; set; }
        public int SharersCountReview { get; set; }
        public decimal RankSeekers { get; set; }
        public int SeekersCountReview { get; set; }
        public int TotalSharedRentals { get; set; }
        public int TotalBorrowedRentals { get; set; }
        public int TotalRentals { get; set; }
        public int TotalCompletedRentals { get; set; }
        public int TotalCancelledRentals { get; set; }
        public decimal CompletedPercentage { get; set; }
        public decimal CancelledPercentage { get; set; }
        public ListViewModel<ReviewViewModel> SharersReviews { get; set; }
        public ListViewModel<ReviewViewModel> SeekersReviews { get; set; }
        public ListViewModel<UserProfileGoodViewModel> Listings { get; set; }
    }
}