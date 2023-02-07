using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodPropertyType : AuditEntity
    {
        public static string _TableName = "d_good_property_type";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual string Name { get; set; }
    }
}