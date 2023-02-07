using Apeek.Entities.Interfaces;
namespace Momentarily.Entities.Entities
{
    public class PinPaymentCustomerMap : AuditEntityMap<PinPaymentCustomer>
    {
        public PinPaymentCustomerMap()
        {
            Table("p_pinpayment_customer");
            Id(x => x.Id, "user_id").GeneratedBy.Assigned();
            Map(x => x.CustomerToken, "customer_token");
        }
    }
}