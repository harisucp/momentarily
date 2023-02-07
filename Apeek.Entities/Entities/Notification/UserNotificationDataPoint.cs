namespace Apeek.Entities.Entities.Notification
{
    public class UserNotificationDataPoint
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string OldValue { get; set; }
        public virtual string NewValue { get; set; }
        public virtual int UserNotificationId { get; set; }
    }
}