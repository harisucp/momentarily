namespace Apeek.Entities.Entities
{
    public class Dns
    {
        public static string _TableName = "s_dns";
        public virtual string TableName { get { return _TableName; } }
        public virtual int DnsId { get; set; } 
        public virtual string Name { get; set; } 
        public virtual int DefaultLangId { get; set; } 
        public virtual bool IsDefault { get; set; } 
    }
}