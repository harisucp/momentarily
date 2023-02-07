using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momentarily.ViewModels.Models
{
   public class LogEntryViewModel
    {
        public  int Id { get; set; }
        public  string Severity { get; set; }
        public  DateTime CreateDate { get; set; }
        public  string ApplicationName { get; set; }
        public  int? UserId { get; set; }
        public  string SessionId { get; set; }
        public  string IpAddress { get; set; }
        public  string SourceName { get; set; }
        public  string Message { get; set; }
        public  string AppVersion { get; set; }
    }
}
