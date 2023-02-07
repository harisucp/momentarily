using System;

namespace Apeek.Entities.Entities.Notification
{
    public class UserNotificationHistory
    {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual string UserEmail { get; set; }
        public virtual int Status { get; set; }
        public virtual int UserNotificatioId { get; set; }
        public virtual DateTime CreateDate { get; set; }


        public UserNotificationHistory()
        {
            CreateDate = DateTime.Now;
        }
    }
}