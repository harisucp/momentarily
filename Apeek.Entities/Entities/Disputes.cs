using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
   public class Disputes : AuditEntity
    {
        public static string _TableName = "c_disputes";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int RequestId { get; set; }
        public virtual int DisputeCreatedBy { get; set; }
        public virtual int LastStatus { get; set; }
        public virtual int Reason { get; set; }
        public virtual string Description { get; set; }

    }
}
