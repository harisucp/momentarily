using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Entities
{
    public class Service : AuditEntityUt, IEntityTree
    {
        public static string _TableName = "c_service";
        public override string TableName { get { return _TableName; } }
        public virtual int? ParentId { get { return Parent != null ? Parent.Id : (int?) null; } set{throw new NotImplementedException();} }
        public virtual bool IsRoot { get; set; }
        public virtual bool ShowTags { get; set; }
        public override int Id { get; set; }
        public virtual int Status { get; set; }
        public virtual bool Hidden { get; set; }
        public virtual Service Parent { get; set; }
        public virtual int Type { get; set; }
        public Service()
        {
            Type = ServiceType.Service;
        }
    }
    public class ServiceType
    {
        public const int Service = 1;
        public const int Category = 2;
        public const int CategoryService = 3;
        public static bool IsService(int type)
        {
            return type == Service || type == CategoryService;
        }
        public static List<SelectListItem> Items()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem(){Text = "Service", Value = Service.ToString()},
                new SelectListItem(){Text = "Category", Value = Category.ToString()},
                new SelectListItem(){Text = "Category/Service", Value = CategoryService.ToString()}
            };
        }
    }
    public class ServiceStatus
    {
        public const int NotVerified = 0;
        public const int Verified = 1;
        public const int Rejected = 2;
        public static List<SelectListItem> Items()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem(){Text = "NotVerified", Value = NotVerified.ToString()},
                new SelectListItem(){Text = "Verified", Value = Verified.ToString()},
                new SelectListItem(){Text = "Rejected", Value = Rejected.ToString()}
            };
        }
    }
    public class ServiceTree
    {
        public static string TableName = "c_service_tree";
    }
}