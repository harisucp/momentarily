using FluentNHibernate.Mapping;
using Apeek.Entities.Entities;
namespace Apeek.Entities.Mappings
{
    public class LogEntryMap : ClassMap<LogEntry>
    {
        public LogEntryMap()
        {
            Table("s_log");
            Id(x => x.Id, "log_id");
            Map(x => x.ApplicationName, "application_name");
            Map(x => x.SourceName, "source_name");
            Map(x => x.Severity, "severity");
            Map(x => x.CreateDate, "create_date");
            Map(x => x.UserId, "user_id");
            Map(x => x.SessionId, "session_id");
            Map(x => x.IpAddress, "ipaddress");
            Map(x => x.Message, "message");
            Map(x => x.AppVersion, "app_version");
        }
    }
}