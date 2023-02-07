using Apeek.Entities.Entities;
namespace Apeek.ViewModels.Models
{
    public class BookingRequestModel
    {
        public GoodRequest GoodRequest { get; set; }
        public Good Good { get; set; }
        public User UserSeller { get; set; }
        public User UserBuyer { get; set; }
    }
}
