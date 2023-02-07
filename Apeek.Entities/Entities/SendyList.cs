using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
   public class SendyList : AuditEntity
    {
        public static string _TableName = "c_sendy_list";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual string ListID { get; set; }
        public virtual bool IsCampaignSend { get; set; }
        public virtual string ListName { get; set; }
    }
}
