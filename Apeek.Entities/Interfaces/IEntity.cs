namespace Apeek.Entities.Interfaces
{
    public interface IEntity
    {
        int Id { get; set; }
        string TableName { get; }
    }
    public interface IEntityTree : IEntity
    {
        int? ParentId { get; set; }
        bool IsRoot { get; set; }
    }
    public interface IDescription
    {
        string Description { get; set; }
    }
}
