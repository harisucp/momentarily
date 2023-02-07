namespace Momentarily.ViewModels.Models.Braintree
{
    public class CreatePurchaseFailModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public CreatePurchaseFailModel()
        {
            IsSuccess = false;
            Message = string.Empty;
        }
    }
}
