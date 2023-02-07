using System;
namespace Apeek.ViewModels.Models.Impl
{
    public class ReviewViewModel
    {
        public string Message { get; set; }
        public decimal Rank { get; set; }
        public string ReviewerImage { get; set; }
        public string ReviewerName { get; set; }
        public DateTime Date { get; set; }
    }
}