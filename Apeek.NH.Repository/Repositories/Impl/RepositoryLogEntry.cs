using Apeek.Common.Configuration;
using Apeek.Common.HttpContextImpl;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



using Apeek.Common.Controllers;
using Apeek.Common.Extensions;
using Apeek.Logger;
using System.Data.SqlClient;
using Apeek.Common;
using Apeek.Common.Logger;

namespace Apeek.NH.Repository.Repositories.Impl
{
   public class RepositoryLogEntry : Repository<LogEntry>, IRepositoryLogEntry
    {
       
        public bool SaveLogs(string logSource, string message, string serverity)
        {

            bool result = false;
            try
            {

            var logEntry = new LogEntry();
            logEntry.ApplicationName = AppSettings.GetInstance().ApplicationName;
            logEntry.Message = message;
            logEntry.SourceName = logSource;
                logEntry.Severity = serverity;
                logEntry.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                using (var c = new SqlConnection(AppSettings.GetInstance().ConnectionStringMysql))
                {
                    c.Open();
                    var command = new SqlCommand(@"Insert into s_log 
                    (application_name,source_name,severity,create_date,user_id,session_id,ipaddress,message,app_version)  values
                    (@application_name,@source_name,@severity,@create_date,@user_id,@session_id,@ipaddress,@message, @app_version)", c);
                    command.Parameters.AddWithValue("@application_name", logEntry.ApplicationName);
                    command.Parameters.AddWithValue("@source_name", logEntry.SourceName);
                    command.Parameters.AddWithValue("@severity", logEntry.Severity);
                    command.Parameters.AddWithValue("@create_date", DateTime.Now);
                    command.Parameters.AddWithValue("@user_id", 0);
                    command.Parameters.AddWithValue("@session_id", "");
                    command.Parameters.AddWithValue("@ipaddress", "");
                    command.Parameters.AddWithValue("@message", logEntry.Message);
                    command.Parameters.AddWithValue("@app_version", logEntry.AppVersion);
                    for (int i = 0; i < command.Parameters.Count; i++)
                    {
                        if (command.Parameters[i].Value == null)
                            command.Parameters[i].Value = DBNull.Value;
                    }
                    command.ExecuteNonQuery();
                }
                result = true;

                #region MyRegion
                //logEntry.SessionId = HomeController.SessionId;
                //if (HttpContextFactory.Current != null)
                //{
                //    if (HttpContextFactory.Current.Session != null)
                //    {
                //        logEntry.SessionId = HttpContextFactory.Current.Session.SessionID;
                //        if (HttpContextFactory.Current.Request != null)
                //            logEntry.IpAddress = HttpContextFactory.Current.Request.ServerVariables["REMOTE_ADDR"];
                //    }
                //    if (ContextService.AuthenticatedUser != null)
                //        logEntry.UserId = ContextService.AuthenticatedUser.UserId;
                //}
                #endregion

            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogError(LogSource.PayPalPaymentService, string.Format("Save log not insert: {0}", ex));
            }
            return result;
        }
    }
}
