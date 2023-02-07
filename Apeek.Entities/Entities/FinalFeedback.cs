using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
        public class FinalFeedback : AuditEntity
    {
        public static string _TableName = "c_final_feedback";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int RequestId { get; set; }
        public virtual bool NoIssue { get; set; }
        public virtual bool Late { get; set; }
        public virtual bool Damaged { get; set; }
        public virtual DateTime ReturnDate { get; set; }
        public virtual string ReturnTime { get; set; }
        public virtual double Claim { get; set; }
        public virtual string Description { get; set; }
    }
}
