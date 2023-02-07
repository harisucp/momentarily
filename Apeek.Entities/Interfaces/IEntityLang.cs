namespace Apeek.Entities.Interfaces
{
    public interface IEntityLang : IEntity
    {
        int Lang_Id { get; set; }
        IEntity Entity { get; set; }
        IEntityLang CreateNewBaseOnThis(int langId);
    }
}