using System;
using Apeek.Common;
using Apeek.Common.EventManager.Definitions;
using Apeek.Common.Logger;
using Momentarily.Cron;
using Momentarily.EventManager.TaskExecutor;
namespace Momentarily.EventManager.Tasks
{
    public abstract class EventManagerTask
    {
        private EventManagerTaskExecutor _executor;
        protected object _processTaskLocker = new object();
        //TODO Delete when time will come
        //For debug purposes only
        public virtual bool IsEnabled
        {
            get { return true; }
        }
        public EventManagerTaskExecutor Executor
        {
            get { return _executor; }
            set { _executor = value; }
        }
        //        public virtual EventManagerTaskExecutor GetExecutor(ICronObject cron, int taskId,
        //            UserEmailNotifications emailType)
        //        {
        //        }
        public abstract TaskTypeIds TaskId { get; }
        //public abstract UserEmailNotifications EmailType { get; }
        public abstract string TaskName { get; }
        public abstract string CronExpression { get; }
        protected abstract void Process(ICronObject cronobject);
        public void ProcessTask(ICronObject cronobject)
        {
            //when a cron is started it is triggered to identify the next time
            //it should not be processed
            if (cronobject.IsFirstTrigger)
            {
                cronobject.IsFirstTrigger = false;
                //TODO Uncomment when time will come
                //comment for debug purposes only
                return;
            }
            Ioc.Get<IDbLogger>().LogMessage(LogSource.EventManager, string.Format("{0} is started working", TaskName));
            try
            {
                Process(cronobject);
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogException(LogSource.EventManager, "Error when processing the task", ex);
            }
            Ioc.Get<IDbLogger>().LogMessage(LogSource.EventManager, string.Format("{0} is finished working", TaskName));
        }
    }
}
