using System;
using System.Collections.Generic;
using Apeek.Common;
using Apeek.Common.Logger;
using Momentarily.Cron;
using Momentarily.EventManager.Tasks;
namespace Momentarily.EventManager
{
    public class CronFactory
    {
        public ICronObject StartCron<T>() where T : EventManagerTask, new()
        {
            //http://blog.bobcravens.com/2009/10/an-event-based-cron-scheduled-job-in-c/
            // Create the cron schedule.
            var eventManagerTask = new T();
            //TODO Remove when time will come
            //For debug purposes only
            if (!eventManagerTask.IsEnabled)
            {
                return null;
            }
            try
            {
                var cronSchedule = CronSchedule.Parse(eventManagerTask.CronExpression);
                var cronSchedules = new List<CronSchedule> { cronSchedule };
                // Create the data context for the cron object.
                var dc = new CronObjectDataContext
                {
                    Object = eventManagerTask,
                    CronSchedules = cronSchedules,
                    LastTrigger = DateTime.MinValue
                };
                // Create the cron object.
//                var cron = new CronObject(dc);
                var cron = CreateCronObject(dc);
                // Register for events.
                //TODO Test this
                cron.OnCronTrigger += eventManagerTask.ProcessTask;
                // Start the cron job.
                cron.Start();
                return cron;
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogException(LogSource.EventManager, ex);
            }
            return null;
        }
        public virtual ICronObject CreateCronObject(CronObjectDataContext dc)
        {
            return new CronObject(dc);
        }
    }
}
