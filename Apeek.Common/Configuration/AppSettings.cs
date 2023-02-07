using System;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
namespace Apeek.Common.Configuration
{
    public class AppSettings : IAppSettings
    {
        //public const string ConnectionStringMySqlKey = "tophandsmysql";
        //private const string ConnectionStringTestKey = "tophandsTest";
        public const string ConnectionStringSqlKey = "Mssqlconnectionstring";
        private const string ConnectionStringTestKey = "TestMssqlconnectionstring";
        private const string LogMessageKey = "LogMessage";
        private const string SendErrorsToAdminEmailKey = "SendErrorsToAdminEmail";
        private const string AllowLocationQueryStringKey = "AllowLocationQueryString";
        private const string AppDataDirectoryKey = "AppDataDirectory";
        private const string BinDirectory = "Bin";
        private const string RunDataTrackerKey = "RunDataTracker";
        private const string RunTasksKey = "RunTasks";
        private const string ApplicationNameKey = "ApplicationName";
        private const string EventManagerWebApiUrlKey = "EventManagerWebApiUrl";
        private const string ModulesNameKey = "Module";
        private const string ImageLocalStoragePathNameKey = "ImageLocalStoragePath";
        private const string GoodImageLocalStoragePathNameKey = "GoodImageLocalStoragePath";
        private const string GoodImageCovidLocalStoragePathNameKey = "GoodImageCovidLocalStoragePath";
        
        public struct ConnectionStringKeys
        {
            public const string HostKey = "Server";
            public const string DataBaseKey = "Database";
            public const string UserKey = "User ID";
            public const string PasswordKey = "Password";
        }
        private string _connectionStringMysql;
        private string _connectionStringTest;
        private bool _sendErrorsToAdminEmail;
        private bool _runDataTracker;
        private bool _runTasks;
        private string _applicationName;
        private List<string> _modulesName;
        private static AppSettings instance;
        private bool _logMessage;
        private bool _allowLocationQueryString;
        private string _appDataDirectory;
        private string _eventManagerWebApiUrl;
        private string _imageLocalStoragePath;
        private string _goodimageLocalStoragePath;
        private string _goodimageCovidLocalStoragePath;
        private bool _default_runTasks = false;
        private bool _default_allowLocationQueryString = false;
        private string _default_applicationName = "momentarily";
        public static AppSettings GetInstance()
        {
            if(instance == null)
            {
                instance = new AppSettings();
            }
            return instance;
        }
        private AppSettings()
        {
            bool success = bool.TryParse(ConfigurationManager.AppSettings[LogMessageKey], out _logMessage);
            if (!success)
            {
                //Ioc.Get<DbLogger>().LogWarning(LogSource.Configuration, string.Format("Value [{0}] was not found", LogMessageKey));
                _logMessage = false;
            }
            success = bool.TryParse(ConfigurationManager.AppSettings[RunDataTrackerKey], out _runDataTracker);
            if (!success)
            {
                //Ioc.Get<DbLogger>().LogWarning(LogSource.Configuration, string.Format("Value [{0}] was not found", RunDataTrackerKey));
                _runDataTracker = false;
            }
            success = bool.TryParse(ConfigurationManager.AppSettings[RunTasksKey], out _runTasks);
            if (!success)
            {
                //Ioc.Get<DbLogger>().LogWarning(LogSource.Configuration, string.Format("Value [{0}] was not found", RunDataTrackerKey));
                _runTasks = _default_runTasks;
            }
            success = bool.TryParse(ConfigurationManager.AppSettings[AllowLocationQueryStringKey], out _allowLocationQueryString);
            if (!success)
            {
                _allowLocationQueryString = _default_allowLocationQueryString;
            }
            success = bool.TryParse(ConfigurationManager.AppSettings[SendErrorsToAdminEmailKey], out _sendErrorsToAdminEmail);
            if (!success)
            {
                //Ioc.Get<DbLogger>().LogWarning(LogSource.Configuration, string.Format("Value [{0}] was not found", SendErrorsToAdminEmailKey));
                _sendErrorsToAdminEmail = false;
            }
            //AppdataDirectory
            _appDataDirectory = ConfigurationManager.AppSettings[AppDataDirectoryKey];
            if (string.IsNullOrEmpty(_appDataDirectory))
            {
                _appDataDirectory = string.Format(@"{0}", Path.Combine(BaseDirectory, "App_Data"));
            }
            var binDirectory = ConfigurationManager.AppSettings[BinDirectory];
            if (!string.IsNullOrEmpty(binDirectory))
            {
                _iocScanDirectory = binDirectory;
            }
            if (!Directory.Exists(_appDataDirectory))
            {
                try
                {
                    //try create App_Data directory.
                    Directory.CreateDirectory(_appDataDirectory);
                }
                catch
                {
                    // ignored
                }
                if (!Directory.Exists(_appDataDirectory))
                {
                    throw new Exception(string.Format("Incorrect path to [App_Data]: {0}", _appDataDirectory));
                }
            }
            //ApplicationName
            _applicationName = ConfigurationManager.AppSettings[ApplicationNameKey];
            if (string.IsNullOrEmpty(_applicationName))
            {
                _applicationName = _default_applicationName;
            }
            var modules = ConfigurationManager.AppSettings[ModulesNameKey];
            if (string.IsNullOrEmpty(modules))
            {
                _modulesName = new List<string>() {"Apeek"};
            }
            else
            {
            _modulesName = ConfigurationManager.AppSettings[ModulesNameKey].Split(',').ToList();
            }
            _imageLocalStoragePath = ConfigurationManager.AppSettings[ImageLocalStoragePathNameKey];
            _goodimageLocalStoragePath = ConfigurationManager.AppSettings[GoodImageLocalStoragePathNameKey];
            _goodimageCovidLocalStoragePath = ConfigurationManager.AppSettings[GoodImageCovidLocalStoragePathNameKey];
            //EventManagerWebApiUrl
            _eventManagerWebApiUrl = ConfigurationManager.AppSettings[EventManagerWebApiUrlKey];
            if (string.IsNullOrEmpty(_eventManagerWebApiUrl))
            {
                _eventManagerWebApiUrl = null;
            }
            if (ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().Any(connStr => connStr.Name == ConnectionStringTestKey))
            {
                _connectionStringTest = ConfigurationManager.ConnectionStrings[ConnectionStringTestKey].ConnectionString;
            }
            if (ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().Any(connStr => connStr.Name == ConnectionStringSqlKey))
            {
                _connectionStringMysql = ConfigurationManager.ConnectionStrings[ConnectionStringSqlKey].ConnectionString;
                DbConnectionStringBuilder connectionStringbuilder = new DbConnectionStringBuilder();
                connectionStringbuilder.ConnectionString = _connectionStringMysql;
            }
            else
            {
                //if we have test connection string it means we run tests and do not nedd throw exception
                if (string.IsNullOrWhiteSpace(_connectionStringTest))
                {
                    var ex = new Exception("No connection string defined");
                    //Ioc.Get<DbLogger>().LogException(LogSource.Configuration, ex);
                    throw ex;
                }
            }
        }
        public string DatabaseName
        {
            get {  return new MySqlConnectionStringBuilder(_connectionStringMysql).Database; }
        }
        public string ConnectionStringMysql
        {
            get { return _connectionStringMysql; }
        }
        public string ConnectionStringTest
        {
            get { return _connectionStringTest; }
        }
        public string BaseDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }
        public string SiteMapDirectory
        {
            get { return string.Format(@"{0}\{1}", BaseDirectory, "SiteMaps"); }
        }
        public List<string> ModulesName {
            get { return _modulesName; }
        }
        public string AppdataDirectory
        {
            get { return _appDataDirectory; }
        }
        public string ImageLocalStoragePath
        {
            get { return _imageLocalStoragePath; }
        }

        public string GoodImageLocalStoragePath
        {
            get { return _goodimageLocalStoragePath; }
        }
        public string GoodImageCovidLocalStoragePath
        {
            get { return _goodimageCovidLocalStoragePath; }
        }
        public string SqlCeDataBasePath
        {
            get { return string.Format(_connectionStringMysql, BaseDirectory); }
        }
        public bool LogMessage
        {
            get { return _logMessage; }
        }
        public bool AllowLocationQueryString
        {
            get { return _allowLocationQueryString; }
        }
        public bool SendErrorsToAdminEmail
        {
            get { return _sendErrorsToAdminEmail; }
        }
        public bool RunDataTracker
        {
            get { return _runDataTracker; }
        }
        public bool RunTasks
        {
            get { return _runTasks; }
        }
        public string ApplicationName
        {
            get { return _applicationName; }
        }
        public string EventManagerWebApiUrl
        {
            get { return _eventManagerWebApiUrl; }
        }
        private string _iocScanDirectory = "bin\\";
        public string IocScanDirectory
        {
            get { return _iocScanDirectory; }
            set { _iocScanDirectory = value; }
        }
    }
}
