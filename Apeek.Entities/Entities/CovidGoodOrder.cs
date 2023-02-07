using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Apeek.Entities.Entities
{
    public class CovidGoodOrder : AuditEntity
    {
        public static string _TableName = "c_covid_good_order";
        #region AuditEntity implementation
        public override string TableName { get { return _TableName; } }
        public override int Id { get; set; }
        #endregion
        public virtual int CovidGoodId { get; set; }
        public virtual double OrderPrice { get; set; }
        public virtual int Quantity { get; set; }
        public virtual double TotalPrice { get; set; }
        public virtual string BuyerEmailId { get; set; }
        public virtual string Description { get; set; }
        public virtual int StatusId { get; set; }
        public virtual double Tax { get; set; }
        public virtual double DeliveryCharge { get; set; }
        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Country { get; set; }
        public virtual string ZipCode { get; set; }
        public virtual string FullName { get; set; }
    }
}
