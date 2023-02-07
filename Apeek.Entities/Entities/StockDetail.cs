using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities
{
    public class StockDetail : AuditEntity
    {
        public static string _TableName = "c_stock_detail";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int CovidGoodId { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string Description { get; set; }
    }
}
