using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
namespace Apeek.Entities.Mappings
{
    public class RequestStatusMap : AuditEntityMap<RequestStatus>
    {
        public RequestStatusMap()
        {
            Table("c_request_status");
            Id(x => x.Id, "id").GeneratedBy.Assigned();
            Map(x => x.StatusName, "status_name");
        }
    }
}
