using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class UserInvitationHistoryMap : ClassMap<UserInvitationHistory>
    {
        public UserInvitationHistoryMap()
        {
            Table("s_user_invitation_history");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.InvitationType, "invitation_type");
            Map(x => x.CreateDate, "create_date");
            Map(x => x.UserResponce, "user_responce");
            Map(x => x.UserResponceCreateDate, "user_responce_create_date");
            Map(x => x.Contact, "contact");
        }
    }
}