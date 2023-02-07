using System;
using System.Diagnostics;
using Apeek.Common.Logger;
using Apeek.Common.Controllers;
namespace Apeek.Common.Diagnostics
{
    public class Watcher : IDisposable
    {
        private string _name;
        private Stopwatch _stopwatch;
        private Watcher(string name)
        {
#if DEBUG
            _name = name;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
#endif
        }
        public static Watcher Start(string name)
        {
            var watcher = new Watcher(name);
            return watcher;
        }
        public void Pause()
        {
            _stopwatch.Stop();
        }
        public void Stop()
        {
#if DEBUG
            _stopwatch.Stop();
            LogTime();
#endif
        }
        protected void LogTime()
        {
            Ioc.Get<DbLogger>().LogMessage(LogSource.WATCHER, string.Format("{0} - {1}ms",_name,_stopwatch.ElapsedMilliseconds));
        }
        public void Dispose()
        {
            Stop();
        }
    }
}