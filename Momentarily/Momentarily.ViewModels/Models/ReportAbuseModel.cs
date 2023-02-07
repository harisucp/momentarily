using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momentarily.ViewModels.Models
{
   public class ReportAbuseModel
    {
        public int Id { get; set; }
        public  int GoodId { get; set; }
        public  int UserId { get; set; }
        public string ItemName { get; set; }
        public string UserName { get; set; }
        public  int count { get; set; }
        public  int GlobalCodeId { get; set; }
        public string GlobalCodeName { get; set; }
        public  string Description { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public  DateTime CreateDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public  DateTime ModDate { get; set; }
        public  int ModBy { get; set; }
        public  int CreateBy { get; set; }
    }

    public class ReportAbuseVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int GoodId { get; set; }
        public string ItemName { get; set; }
        public int count { get; set; }
        public int GlobalCodeId { get; set; }
        public string GlobalCodeName { get; set; }
        public string Description { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreateDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ModDate { get; set; }
        public int ModBy { get; set; }
        public int CreateBy { get; set; }
        public int TotalCount { get; set; }

        public List<ReportAbuseModel> reportAbuseDetalList { get; set; }
    }
}
