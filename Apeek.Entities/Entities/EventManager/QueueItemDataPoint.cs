namespace Apeek.Entities.Entities.EventManager
{
    public class QueueItemDataPoint
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
        public virtual int QueueId { get; set; }
    }
}