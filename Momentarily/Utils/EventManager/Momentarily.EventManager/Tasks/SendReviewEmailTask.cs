using Apeek.Common.EventManager.Definitions;
using Momentarily.Cron;
using Momentarily.EventManager.TaskExecutor;
namespace Momentarily.EventManager.Tasks
{
    public class SendReviewEmailTask : EventManagerTask
    {
        //TODO delete when time will come
        //for debug purposes only
        public override bool IsEnabled
        {
            get { return true; }
        }
        public override TaskTypeIds TaskId
        {
            get { return TaskTypeIds.SendReviewEmailTask; }
        }
        public override string TaskName
        {
            get { return "Send review emails"; }
        }
        public override string CronExpression
        {
            get { return "15 * * * *"; }
        }
        protected override void Process(ICronObject cronobject)
        {
            //lock is used here to be sure that user is not targeted by task in multiple threads
            lock (_processTaskLocker)
            {
                var _sendReviewEmail = new SendReviewEmailTaskExecutor(cronobject, (int)TaskId);
                _sendReviewEmail.Execute();
            }
        }
    }
}