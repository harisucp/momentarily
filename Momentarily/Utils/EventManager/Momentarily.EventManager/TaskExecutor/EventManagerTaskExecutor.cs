using Momentarily.Cron;
namespace Momentarily.EventManager.TaskExecutor
{
    public abstract class EventManagerTaskExecutor
    {
        protected ICronObject _cronobject;
        protected int _taskId;
        //protected UserEmailNotifications _emailType;
        //protected EventManagerTaskExecutor(ICronObject cronobject, int taskId, UserEmailNotifications emailType)
        protected EventManagerTaskExecutor(ICronObject cronobject, int taskId)
        {
            _cronobject = cronobject;
            _taskId = taskId;
            //_emailType = emailType;
        }
        public abstract void Execute();
    }
}
