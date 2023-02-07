namespace Apeek.Entities.Entities
{
    public interface IUserImg
    {
        int UserId { get; set; }
        int Type { get; set; }
        int Sequence { get; set; }
        string FileName { get; set; }
    }
}