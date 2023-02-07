namespace Apeek.Entities.Entities.EventManager
{
    public class EventManagerTaskConfig
    {
        public virtual int Id { get; set; }
        public virtual string TaskName { get; set; }
        public virtual string CronExpression { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual int? CountToProcess { get; set; }
    }
}