using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
   public class CancelledRequest : AuditEntity
    {
        public static string _TableName = "c_cancel_request";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int UserId { get; set; }
        public virtual int RequestId { get; set; }
    }
}
