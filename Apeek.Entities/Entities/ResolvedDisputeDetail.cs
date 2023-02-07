using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
    public class ResolvedDisputeDetail : AuditEntity
    {
        public static string _TableName = "c_resolved_dispute_detail";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int RequestId { get; set; }
        public virtual int DisputeId { get; set; }
        public virtual double TotalPaidAmount { get; set; }
        public virtual double BorrowerShare { get; set; }
        public virtual double OwnerShare { get; set; }
        public virtual double MomentarilyShare { get; set; }
        public virtual double AmountLimitToPay { get; set; }
        public virtual int FinalRentalReason { get; set; }
        public virtual string Description { get; set; }

    }
}
