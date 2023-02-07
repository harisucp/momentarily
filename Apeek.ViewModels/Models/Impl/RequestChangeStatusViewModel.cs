namespace Apeek.ViewModels.Models
{
    public class RequestChangeStatusViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public int ReasonId { get; set; }
        public string Message { get; set; }
    }
}