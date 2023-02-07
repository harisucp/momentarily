using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class ResolvedDisputeDetailMap : AuditEntityMap<ResolvedDisputeDetail>
    {
        public ResolvedDisputeDetailMap()
        {
            Table("c_resolved_dispute_detail");
            Id(x => x.Id, "id");
            Map(x => x.DisputeId, "dispute_id");
            Map(x => x.RequestId, "request_id");
            Map(x => x.TotalPaidAmount, "total_paid_amount");
            Map(x => x.BorrowerShare, "borrower_share");
            Map(x => x.OwnerShare, "owner_share");
            Map(x => x.MomentarilyShare, "momentarily_share");
            Map(x => x.AmountLimitToPay, "amount_limit_to_pay");
            Map(x => x.FinalRentalReason, "final_rental_reason");
            Map(x => x.Description, "description");
        }
    }
}