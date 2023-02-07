using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class HistoryPhoneNumberMap : ClassMap<HistoryPhoneNumber>
    {
        public HistoryPhoneNumberMap()
        {
            Table("c_history_phone_number");
            Id(x => x.Id, "history_phone_number_id");
            Map(x => x.PhoneNumber, "phone_number");
            Map(x => x.HistoryUserId, "history_user_id");
        }
    }
}