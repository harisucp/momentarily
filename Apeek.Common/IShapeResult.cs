namespace Apeek.Common
{
    public interface IShapeResult
    {
        bool IsError { get; set; }
        string Message { get; set; }
    }
}
