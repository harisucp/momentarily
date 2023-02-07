using System;
namespace Apeek.Entities.Entities
{
    public class LogEntry
    {
        public virtual int Id { get; set; }
        public virtual string Severity { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual string ApplicationName { get; set; }
        public virtual int? UserId { get; set; }
        public virtual string SessionId { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual string SourceName { get; set; }
        public virtual string Message { get; set; }
        public virtual string AppVersion { get; set; }
        public LogEntry()
        {
            CreateDate = DateTime.Now;
        }
    }
}