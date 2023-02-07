using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodProperty : AuditEntity
    {
        public static string _TableName = "d_good_property";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual string Name { get; set; }
        public virtual int TypeId { get; set; }
    }
}