using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Models.Impl
{
    public class GoodRequestReviews
    {
        public int GoodRequestId { get; set; }
        public GoodBookingRank SeekersReview { get; set; }
        public GoodBookingRank SharersReview { get; set; }
    }
}