using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class Setting : IEntity
    {
        public static string _TableName = "s_settings";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}