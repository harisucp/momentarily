using Apeek.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momentarily.ViewModels.Models
{
    public class BookingRankViewModal
    {
        public List<BookingRank> RankFromSeekers { get; set;}
        public List<BookingRank> RankFromSharers { get; set; }
    }

    public class BookingRank
    {
        public int Id { get; set; }
        public int GoodRequestId { get; set; }
        public int GoodId { get; set; }
        public string GoodName { get; set; }
        public int SharerId { get; set; }
        public string SharerName { get; set; }
        public int SeekerId { get; set; }
        public string SeekerName { get; set; }
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; }
        public int Rank { get; set; }
        public string Message { get; set; }
        public User Reviewer { get; set; }
        public int UserId { get; set; }
    }
}
