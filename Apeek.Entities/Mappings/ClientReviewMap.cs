using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;

namespace Apeek.Entities.Mappings
{
    public class ClientReviewMap : AuditEntityUtMap<ClientReview>
    {
        public ClientReviewMap()
        {
            Table("c_client_review");

            Id(x => x.Id, "client_review_id");
            Map(x => x.IsRoot, "is_root");
            Map(x => x.UserId, "user_id");
            Map(x => x.ServiceId, "service_id");
            Map(x => x.ClientId, "client_id");
            Map(x => x.Text, "text");
            Map(x => x.UserAccAssId, "user_acc_ass_id");
        }
    }
}