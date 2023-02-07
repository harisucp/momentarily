using Apeek.Common.EventManager.Definitions;
using Momentarily.Cron;
using Momentarily.EventManager.TaskExecutor;

namespace Momentarily.EventManager.Tasks
{
    public class VoidAuthorizationFundsTask : EventManagerTask
    {
        //TODO delete when time will come
        //for debug purposes only
        public override bool IsEnabled
        {
            get { return false; }
        }

        public override TaskTypeIds TaskId
        {
            get { return TaskTypeIds.VoidAuthorizeTask; }
        }
        

        public override string TaskName
        {
            get { return "VoidAuthorize"; }
        }

        public override string CronExpression
        {
            //TODO: for testing run every 10 minute
            get { return "0-59/10 * * * *"; }
        }

        protected override void Process(ICronObject cronobject)
        {
            //lock is used here to be sure that user is not targeted by task in multiple threads
            lock (_processTaskLocker)
            {
                var _voidAuthorizationFundsTaskExecutor = new VoidAuthorizationFundsTaskExecutor(cronobject, (int)TaskId);
                _voidAuthorizationFundsTaskExecutor.Execute();
            }
        }
    }
}