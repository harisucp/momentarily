using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
    public class FAQ : AuditEntity
    {
        public static string _TableName = "c_faq";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual string Question { get; set; }
        public virtual string Answer { get; set; }
        public virtual int Type { get; set; }
    }
}
