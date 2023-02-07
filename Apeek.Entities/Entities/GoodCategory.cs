using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodCategory : AuditEntity
    {
        public static string _TableName = "c_good_category";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int GoodId { get; set; }
        public virtual int CategoryId { get; set; }

       
    }
}