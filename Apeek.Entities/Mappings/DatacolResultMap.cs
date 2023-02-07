using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class DatacolResultMap : ClassMap<DatacolResult>
    {
        public DatacolResultMap()
        {
            Table("s_datacol_results");
            Id(x => x.Id, "id");
            Map(x => x.ServiceName, "service_name");
            Map(x => x.RubricName, "rubric_name");
            Map(x => x.CityName, "city_name");
            Map(x => x.AddressLine, "address_line");
            Map(x => x.Description, "descr");
            Map(x => x.UserName, "user_name");
            Map(x => x.Price, "price");
            Map(x => x.PhoneNums, "phone_nums");
            Map(x => x.SourceId, "source_id");
            Map(x => x.SourceUrl, "source_url");
            Map(x => x.SourceName, "source_name");
            Map(x => x.SourceCreateDate, "source_create_date");
            Map(x => x.ProcessStatus, "process_status");
            Map(x => x.ErrorReason, "error_reason");
            Map(x => x.CreateDate, "create_date");
            Map(x => x.ServiceParentIdForNewServices, "service_parent_id");
        }
    }
}