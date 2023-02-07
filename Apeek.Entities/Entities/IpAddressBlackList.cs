using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class IpAddressBlackList : IEntity
    {
        public static string _TableName = "s_ip_address_black_list";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual string Ip { get; set; }
    }
}