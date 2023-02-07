namespace Apeek.ViewModels.Models.Impl
{
    public class ApeekPayout
    {
        public int UserId { get; set; }
        public int GoodId { get; set; }
        public string GoodName { get; set; }
        public string Description { get; set; }
        public int GoodRequestId { get; set; }
        public int GoodRequestUserId { get; set; }
        public string RecipientToken { get; set; }
        public string Email { get; set; }
        public double Amount { get; set; }
    }
}