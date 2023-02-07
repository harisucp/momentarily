using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Entities.Interfaces;

namespace Apeek.Entities.Entities.Notification
{
    public class UserNotification : AuditEntity
    {
        public override int Id { get; set; }

        public override string TableName
        {
            get { return "em_user_notification"; }
        }

        public virtual int SourceId { get; set; }
        public virtual int EventType { get; set; }/*Service Changed*/
        public virtual int NotificationType { get; set; }/*sms, email*/
        public virtual int Status { get; set; }/*processed, inprogress, not processed*/

        public virtual string ServiceName { get; set; }

        public virtual IList<UserNotificationDataPoint> DataPoints { get; set; }
        public virtual IList<UserNotificationDataPoint> DataPointsToDelete { get; set; }

        public UserNotification()
        {
            CreateDate = DateTime.Now;
            DataPoints = new List<UserNotificationDataPoint>();
            DataPointsToDelete = new List<UserNotificationDataPoint>();
        }

        public virtual void MergeDataPoints(IList<UserNotificationDataPoint> newDataPoints)
        {
            foreach (var newDataPoint in newDataPoints)
            {
                var dp = DataPoints.ToList().Find(x => x.Name == newDataPoint.Name);
                if (dp != null)
                {
                    //if we revert to old value we have to remove datapoint
                    if (string.Compare(dp.OldValue, newDataPoint.NewValue, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        DataPoints.Remove(dp);
                        DataPointsToDelete.Add(dp);
                    }
                    else
                        dp.NewValue = newDataPoint.NewValue;
                }
                else
                {
                    DataPoints.Add(newDataPoint);
                }
            }
        }
    }
}