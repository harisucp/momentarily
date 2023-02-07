using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class ClientServiceRequestMap : ClassMap<ClientServiceRequest>
    {
        public ClientServiceRequestMap()
        {
            Table("c_client_service_request");
            Id(x => x.RequestId, "request_id");
            Map(x => x.ClientId, "client_id");
            Map(x => x.UserId, "user_id");
            Map(x => x.ServiceId, "service_id");
            Map(x => x.LocationId, "location_id");
            Map(x => x.CreateDate, "create_date");
            Map(x => x.IsClientEmailSent, "is_client_email_sent");
            Map(x => x.IsRequestComplete, "is_request_complete");
            Map(x => x.RequestType, "request_type");
            Map(x => x.PageName, "page_name");
        }
    }
}