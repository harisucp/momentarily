using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using FluentNHibernate.Mapping;
namespace Apeek.Entities.Mappings
{
    public class PinPaymentCardMap : AuditEntityMap<PinPaymentCard>
    {
        public PinPaymentCardMap()
        {
            Table("p_pinpayment_card");
            Id(x => x.Id, "id");
            Map(x => x.CardToken, "card_token");
            Map(x => x.DisplayNumber, "display_number");
            Map(x => x.UserId, "user_id");
        }
    }
}