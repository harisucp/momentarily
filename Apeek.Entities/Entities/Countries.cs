using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
  public  class Countries : AuditEntity
    {
        public static string _TableName = "c_countries";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual string ISO { get; set; }
        public virtual string Name { get; set; }
        public virtual string NiceName { get; set; }
        public virtual char ISO3 { get; set; }
        public virtual int NumCode { get; set; }
        public virtual int PhoneCode { get; set; }
    }
}
