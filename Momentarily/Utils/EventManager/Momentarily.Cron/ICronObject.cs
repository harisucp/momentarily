using System;
namespace Momentarily.Cron
{
    public interface ICronObject
    {
        event CronObject.CronEvent OnCronTrigger;
        event CronObject.CronEvent OnStarted;
        event CronObject.CronEvent OnStopped;
        event CronObject.CronEvent OnThreadAbort;
        bool IsFirstTrigger { get; set; }
        Guid Id { get; }
        object Object { get; }
        DateTime LastTigger { get; }
        bool IsStopRequested { get; }
        bool Start();
        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns></returns>
        bool Stop();
    }
}