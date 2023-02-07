using System;
using System.IO;
using System.Text;
using Apeek.Common.Configuration;
using Apeek.Common.Controllers;
using Apeek.Logger;
namespace Apeek.Common.Logger
{
    public class FileLogger
    {
        private static string Path;
        private static StreamWriter FileWriter;
        public delegate void LoggErrorDelegate(string message, LogSource logSource);
        public static event LoggErrorDelegate OnLoggError;
        private static object _locker = new object();
        static FileLogger()
        {
            InitializeLog();
        }
        private static void InitializeLog()
        {
            Path = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
            FileWriter = new StreamWriter(new FileStream(Path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
        }
        public static void LogError(LogSource logSource, string message)
        {
            message = ContextService.GetErrorModel(message);
            Log(message, MessageType.ERROR, logSource);
            try
            {
                if (OnLoggError != null)
                {
                    lock (_locker)
                    {
                        OnLoggError(message, logSource);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString(), MessageType.ERROR, LogSource.Logger);
            }
        }
        public static void LogMessage(LogSource logSource, string message)
        {
            if (AppSettings.GetInstance().LogMessage)
            {
                Log(message, MessageType.MESSAGE, logSource);
            }
        }
        public static void LogWarning(LogSource logSource, string message)
        {
            Log(message, MessageType.WARNING, logSource);
        }
        public static void LogException(LogSource logSource, Exception ex)
        {
            LogError(logSource, ex.ToString());
        }
        public static void LogException(LogSource logSource, string message, Exception ex)
        {
            message = string.Format("Custom message: {0};\nException: {1}", message, ex);
            LogError(logSource, message);
        }
        public static void Log(string message, MessageType messageType, LogSource logSource)
        {
            message = string.Format("{0} \t{1}({2}): {3}", DateTime.Now, messageType, logSource, message);
            WriteToFile(message);
            LoggerQueue.Add(message);
        }
        protected static void WriteToFile(string message)
        {
            FileWriter.WriteLine(message);
            FileWriter.Flush();
        }
        public static string GetContentFromLogFile()
        {
            string logContent = string.Empty;
            try
            {
                FileWriter.Close();
                logContent = File.ReadAllText(Path);
                InitializeLog();
                return logContent;
            }
            catch (Exception ex)
            {
                LogException(LogSource.Logger, ex);
            }
            return logContent;
        }
        public static void ClearLogFile()
        {
            try
            {
                FileWriter.Close();
                File.Delete(Path);
                InitializeLog();
            }
            catch (Exception ex)
            {
                LogException(LogSource.Logger, ex);
            }
        }
    }
}