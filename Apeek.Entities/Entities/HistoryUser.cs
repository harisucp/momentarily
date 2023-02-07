using System.Collections.Generic;
namespace Apeek.Entities.Entities
{
    public class HistoryUser
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual int? LocationId { get; set; }
        public virtual IList<HistoryPhoneNumber> HistoryPhoneNumbers { get; set; }
    }
}