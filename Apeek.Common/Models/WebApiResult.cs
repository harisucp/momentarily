namespace Apeek.Common.Models
{
    public class WebApiResult<T> : Result<T>
    {
        public WebApiResult(CreateResult result, T obj) : base(result, obj)
        {
        }
        public string Message { get; set; }
    }
}
