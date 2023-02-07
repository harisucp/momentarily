using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class GoodLocation : AuditEntity
    {
        public static string _TableName = "c_good_location";
        public override int Id { get; set; }
        public override string TableName { get { return _TableName; } }
        public virtual int GoodId { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
    }
}
