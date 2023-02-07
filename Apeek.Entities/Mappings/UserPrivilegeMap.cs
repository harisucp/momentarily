using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class UserPrivilegeMap : ClassMap<UserPrivilege>
    {
        public UserPrivilegeMap()
        {
            Table("s_user_privilege");
            Id(x => x.Id, "privilege_id");
            Map(x => x.UserId, "user_id");
            Map(x => x.PrivilegeName, "privilege_name");
            Map(x => x.HasAccess, "has_access");
        }
    }
}
