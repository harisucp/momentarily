using System;
using System.Collections.Generic;

namespace Apeek.Entities.Entities.EventManager
{
    public class QueueItem
    {
        public virtual int Id { get; set; }
        public virtual IList<QueueItemDataPoint> DataPoints { get; set; }
        public virtual string TargetPlugin { get; set; }
        public virtual int IsProcessed { get; set; }
        public virtual DateTime CreateDate { get; set; }

        public QueueItem()
        {
            CreateDate = DateTime.Now;
            DataPoints = new List<QueueItemDataPoint>();
        }
    }
}