using System;
using Apeek.Common.Configuration;
using Apeek.Common.Controllers;
using Apeek.Common.Extensions;
using Apeek.Logger;
using MySql.Data.MySqlClient;
using Apeek.Entities.Entities;
using System.Data.SqlClient;
namespace Apeek.Common.Logger
{
    public class DbLogger : IDbLogger
    {
        public delegate void LoggErrorDelegate(string message, string logSource);
        public static event LoggErrorDelegate OnLoggError;
        public void LogException(LogSource logSource, Exception ex)
        {
            LogError(logSource, ex.ToString());
        }
        public void LogException(LogSource logSource, string message, Exception ex)
        {
            message = string.Format("Custom message: {0};\nException: {1}", message, ex);
            LogError(logSource, message);
        }
        public void LogError(LogSource logSource, string message)
        {
            var logEntry = ContextService.LogEntry(logSource, message);
            LogError(logEntry);
        }
        public void LogError(LogSource logSource, string format, params object[] args)
        {
            LogError(logSource, string.Format(format, args));
        }
        public void LogError(LogEntry logEntry)
        {
            logEntry.Message = ContextService.GetErrorModel(logEntry.Message);
            logEntry.Severity = MessageType.ERROR.ToString();
            Logg(logEntry);
            try
            {
                if (OnLoggError != null)
                {
                    OnLoggError(logEntry.Message, logEntry.SourceName);
                }
            }
            catch (Exception ex)
            {
                Log(LogSource.Logger ,ex.ToString(), MessageType.ERROR);
            }
        }
        public void LogMessage(LogEntry logEntry)
        {
            logEntry.Severity = MessageType.MESSAGE.ToString();
            Logg(logEntry);
        }
        public void Log(LogSource logSource, string message, MessageType messageType)
        {
            var logEntry = ContextService.LogEntry(logSource, message);
            logEntry.Severity = messageType.ToString();
            Logg(logEntry);
        }
        public void LogMessage(LogSource logSource, string message)
        {
            var logEntry = ContextService.LogEntry(logSource, message);
            logEntry.Severity = MessageType.MESSAGE.ToString();
            Logg(logEntry);
        }
        public void LogMessage(LogSource logSource, string format, params object [] args)
        {
            LogMessage(logSource, string.Format(format, args));
        }
        public void LogWarning(LogEntry logEntry)
        {
            logEntry.Severity = MessageType.WARNING.ToString();
            Logg(logEntry);
        }
        public void LogWarning(LogSource logSource, string message)
        {
            var logEntry = ContextService.LogEntry(logSource, message);
            logEntry.Severity = MessageType.WARNING.ToString();
            Logg(logEntry);
        }
        public void LogWarning(LogSource logSource, string format, params object[] args)
        {
            LogWarning(logSource, string.Format(format, args));
        }

        //public void LogMessagePayementProcess(string logSource, string message)
        //{
        //    var logEntry = ContextService.LogEntry(logSource, message);
        //    logEntry.Severity = MessageType.MESSAGE.ToString();
        //    Logg(logEntry);
        //}
        //public void LogMessagePayementProcess(string logSource, string format, params object[] args)
        //{
        //    LogMessagePayementProcess(logSource, string.Format(format, args));
        //}



        private void Logg(LogEntry logEntry)
        {
            try
            {
                using (var c = new SqlConnection(AppSettings.GetInstance().ConnectionStringMysql))
                {
                    c.Open();
                    var command = new SqlCommand(@"Insert into s_log 
                    (application_name,source_name,severity,create_date,user_id,session_id,ipaddress,message,app_version)  values
                    (@application_name,@source_name,@severity,@create_date,@user_id,@session_id,@ipaddress,@message, @app_version)", c);
                    command.Parameters.AddWithValue("@application_name", logEntry.ApplicationName);
                    command.Parameters.AddWithValue("@source_name", logEntry.SourceName);
                    command.Parameters.AddWithValue("@severity", logEntry.Severity);
                    command.Parameters.AddWithValue("@create_date", logEntry.CreateDate);
                    command.Parameters.AddWithValue("@user_id", logEntry.UserId);
                    command.Parameters.AddWithValue("@session_id", logEntry.SessionId);
                    command.Parameters.AddWithValue("@ipaddress", logEntry.IpAddress);
                    command.Parameters.AddWithValue("@message", logEntry.Message);
                    command.Parameters.AddWithValue("@app_version", logEntry.AppVersion);
                    for (int i = 0; i < command.Parameters.Count; i++)
                    {
                        if (command.Parameters[i].Value == null)
                            command.Parameters[i].Value = DBNull.Value;
                    }
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError(LogSource.Logger, string.Format("Cannot log message, excpetion occured: {0}",ex));
                FileLogger.Log(logEntry.Message, EnumHelper.ParseEnum<MessageType>(logEntry.Severity), EnumHelper.ParseEnum<LogSource>(logEntry.SourceName));
            }
            try
            {
                LoggerQueue.Add(logEntry.Message);
            }
            catch { }
        }
    }
}