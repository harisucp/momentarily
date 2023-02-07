using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class PersonServiceMap : ClassMap<PersonService>
    {
        public PersonServiceMap()
        {
            Table("c_user_service");
            CompositeId()
                .KeyReference(x => x.User, "user_id")
                .KeyReference(x => x.Service, "service_id");
            Map(x => x.Price, "price").Nullable();
            Map(x => x.FromPrice, "from_price").Nullable();
            Map(x => x.ToPrice, "to_price").Nullable();
            Map(x => x.RawPrice, "raw_price").Nullable();
            Map(x => x.Metric, "metric").Nullable();
            Map(x => x.Description, "description").Nullable();
            Map(x => x.Header, "header").Nullable();
            // while adding new columns to table add it to sql in RepositoryPersonService.MovePersonsFromServiceToService method
        }
    }
}