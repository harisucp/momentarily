using System.Collections.Generic;
namespace Apeek.ViewModels.Models
{
    public class BookingListViewModel
    {
        public string GoodName { get; set; }
        public List<GoodRequestViewModel> GoodRequests { get; set; }
    }
}