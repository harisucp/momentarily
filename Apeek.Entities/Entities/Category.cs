using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class Category : AuditEntity, IEntityTree
    {
        public static string _TableName { get { return "c_category"; } }
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        #region IEntityTree implementation
        public virtual int? ParentId { get; set; }
        public virtual bool IsRoot { get; set; }
        #endregion
        public virtual string Name { get; set; }
        public virtual string ImageFileName { get; set; }
    }
}