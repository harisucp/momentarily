namespace Apeek.Common.Models
{
    public class Result<T>
    {
        public CreateResult CreateResult { get; set; }
        public T Obj { get; set; }
        public string Message { get; set; }                     
        public Result(CreateResult result, T obj, string message = "")
        {
            CreateResult = result;
            Obj = obj;
            Message = message;
        }
    }
}