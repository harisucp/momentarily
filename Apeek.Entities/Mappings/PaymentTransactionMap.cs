using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class PaymentTransactionMap : AuditEntityMap<PaymentTransaction>
    {
        public PaymentTransactionMap()
        {
            Table("c_payment_transaction");
            Id(x => x.Id, "id");
            Map(x => x.TransactionId, "transaction_id");
            Map(x => x.PayerId, "payer_id");
            Map(x => x.CapureId, "capture_id");
            Map(x => x.GoodRequestId, "good_request_id");
            Map(x => x.Type, "type");
            Map(x => x.Cost, "cost");
            Map(x => x.Commision, "commision");
            Map(x => x.StatusId, "status_id");
            References(x => x.GoodRequest)
                .Column("good_request_id")
                .Not.Update()
                .Not.Insert();
        }
    }
}
