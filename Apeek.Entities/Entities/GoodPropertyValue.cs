using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodPropertyValue : AuditEntity
    {
        public static string _TableName = "c_good_property_value";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int GoodId { get; set; }
        public virtual int GoodPropertyId { get; set; }
        public virtual int? PropertyValueDefinitionId { get; set; }
        public virtual string Value { get; set; }
    }
}