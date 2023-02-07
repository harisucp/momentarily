namespace Apeek.Entities.Entities
{
    public class HistoryPhoneNumber
    {
        public virtual int Id { get; set; }
        public virtual int HistoryUserId { get; set; }
        public virtual string PhoneNumber { get; set; }
    }
}