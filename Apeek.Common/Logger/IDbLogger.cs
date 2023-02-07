using System;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
namespace Apeek.Common.Logger
{
    public interface IDbLogger : IDependency
    {
        void LogError(LogEntry logEntry);
        void LogError(LogSource logSource, string message);
        void LogError(LogSource logSource, string format, params object[] args);
        void LogException(LogSource logSource, Exception ex);
        void LogException(LogSource logSource, string message, Exception ex);
        void LogMessage(LogEntry logEntry);
        void LogMessage(LogSource logSource, string message);
        void LogMessage(LogSource logSource, string format, params object[] args);
        void LogWarning(LogEntry logEntry);
        void LogWarning(LogSource logSource, string message);
        void LogWarning(LogSource logSource, string format, params object[] args);

        //void LogMessagePayementProcess(string logSource, string message);
        //void LogMessagePayementProcess(string logSource, string message, params object[] args);
    }
}
