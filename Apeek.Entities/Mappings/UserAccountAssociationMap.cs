using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class UserAccountAssociationMap : ClassMap<UserAccountAssociation>
    {
        public UserAccountAssociationMap()
        {
            Table("c_user_account_association");
            Id(x => x.Id, "id");
            Map(x => x.UserId, "user_id");
            Map(x => x.ExternalId, "external_id");
            Map(x => x.ProviderName, "provider_name");
            Map(x => x.ExtraData, "extra_data");
            Map(x => x.ImageUrl, "image_url");
        }
    }
}