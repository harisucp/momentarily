using System;
namespace Apeek.ViewModels.Models
{
    public class GoodRequestRankInsertModel
    {
        public int GoodRequestId { get; set; }
        public int GoodId { get; set; }
        public int Sender { get; set; }
        public int Rank { get; set; }
        public string Message { get;set;}
    }
}
