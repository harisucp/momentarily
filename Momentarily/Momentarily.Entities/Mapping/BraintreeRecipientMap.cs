using Apeek.Entities.Interfaces;
namespace Momentarily.Entities.Entities
{
    public class BraintreeRecipientMap : AuditEntityMap<BraintreeRecipient>
    {
        public BraintreeRecipientMap()
        {
            Table("p_braintree_recipient");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.MerchantAccountId, "merchant_account_id");
        }
    }
}