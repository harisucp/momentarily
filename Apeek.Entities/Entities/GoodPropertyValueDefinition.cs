using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodPropertyValueDefinition : AuditEntity
    {
        public static string _TableName = "c_good_property_value_definition";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int GoodPropertyId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
    }
}