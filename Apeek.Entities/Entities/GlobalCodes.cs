using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
   public class GlobalCodes : AuditEntity
    {
        public static string _TableName = "c_global_code";
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        public virtual string GlobalCodeName { get; set; }
        public virtual int GlobalCodeCategoryId { get; set; }

    }
}
