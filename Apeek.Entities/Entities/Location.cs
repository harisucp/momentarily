using System;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class Location : AuditEntityUt, IEntityTree
    {
        public static string _TableName = "d_location";
        public override string TableName { get { return _TableName; } }
        public virtual int? ParentId { get { return Parent != null ? Parent.Id : (int?)null; } set { throw new NotImplementedException(); } }
        public override int Id { get; set; }
        public virtual bool IsRoot { get; set; }
        public virtual bool Hidden { get; set; }
        public virtual bool DisplayInMenu { get; set; }
        public virtual Location Parent { get; set; }
        public virtual bool Verified { get; set; }
        public virtual bool IsDefault { get; set; }
        public Location()
        {
            DisplayInMenu = false;
            Hidden = false;
            IsRoot = false;
            IsDefault = true;
        }
    }
}