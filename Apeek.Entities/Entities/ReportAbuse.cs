using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
  public  class ReportAbuse : AuditEntity, IDescription
    {
        public static string _TableName = "c_report_abuse";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int GoodId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int count { get; set; }
        public virtual int GlobalCodeId { get; set; }
        public virtual string Description { get; set; }
    }
}
