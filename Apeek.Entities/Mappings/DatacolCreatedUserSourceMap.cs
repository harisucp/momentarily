using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class DatacolCreatedUserSourceMap : ClassMap<DatacolCreatedUserSource>
    {
        public DatacolCreatedUserSourceMap()
        {
            Table("s_datacol_created_user_source");
            CompositeId()
                .KeyProperty(x => x.UserId, "user_id")
                .KeyProperty(x => x.DatacolResultId, "datacol_result_id");
        }        
    }
}