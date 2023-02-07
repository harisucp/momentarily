using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class ServiceLang : IEntityLang
    {
        public static string _TableName = "c_service_lang";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual Service Service { get; set; }
        public virtual int Lang_Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }
        public virtual IEntity Entity { get { return Service; } set { Service = (Service)value; } }
        // Not related to entity
        public virtual int ServiceId { get; set; }
        public virtual int Count { get; set; }
        public virtual bool IsUITreeRoot { get; set; }
        public virtual ServiceLang SuperParent { get; set; }
        public virtual string Path { get; set; }
        public ServiceLang()
        {
            Service = new Service();
        }
        public virtual IEntityLang CreateNewBaseOnThis(int langId)
        {
            return new ServiceLang()
            {
                Service = this.Service,
                Lang_Id = langId,
                Name = this.Name
            };
        }
    }
}