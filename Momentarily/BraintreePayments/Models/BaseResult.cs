namespace BraintreePayments.Models
{
    public class BaseResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public BaseResult()
        {
            IsSuccess = false;
            Message = string.Empty;
        }
    }
}
