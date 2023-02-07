using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
   public class CAMasterBulkEmail : AuditEntity
    {
        public static string _TableName = "c_ca_master_bulk";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual string EmailID { get; set; }
        public virtual bool AddedToSendy { get; set; }
        
    }
}
