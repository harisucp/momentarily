using System.Collections.Generic;
using Apeek.Entities.Interfaces;
using Apeek.Entities.Validators;
namespace Apeek.Entities.Entities
{
    public class LocationLang : IEntityLang
    {
        public static string _TableName = "";
        public virtual string TableName { get { return _TableName; } }
        public virtual int Id { get; set; }
        public virtual Location Location { get; set; }
        public virtual int Lang_Id { get; set; }
        [ValidatorCity]
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }
        public virtual string SubDomainUrl { get; set; }
        public virtual IEntity Entity { get { return Location; } set { Location = (Location)value; } }
        // Not related to entity
        public virtual int LocationId { get; set; }
        public virtual int Count { get; set; }
        public virtual bool IsRoot { get; set; }
        public virtual LocationLang SuperParent { get; set; }
        public virtual IEntityLang CreateNewBaseOnThis(int langId)
        {
            return new LocationLang()
            {
                Location = this.Location,
                Lang_Id = langId,
                Name = this.Name
            };
        }
    }
    public class LocationLangDescr
    {
        public virtual int Id { get; set; }
        public virtual int LocationId { get; set; }
        public virtual int Lang_Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string ShortDescription { get; set; }
        public virtual List<ServiceLang> LocationServices { get; set; }
    }
}