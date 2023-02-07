using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
   public class CovidOrderPaymentDetailMap : AuditEntityMap<CovidOrderPaymentDetail>
    {
        public CovidOrderPaymentDetailMap()
        {
            Table("p_paypal_covid_order_detail");
            Id(x => x.Id, "id");
            Map(x => x.CovidOrderId, "covid_order_id");
            Map(x => x.PayId, "pay_id");
            Map(x => x.Intent, "intent");
            Map(x => x.State, "state");
            Map(x => x.Amount, "transaction_amount");
            Map(x => x.Description, "transaction_dsc");
            Map(x => x.InvoiceNumber, "invoice_number");
            Map(x => x.CreateTime, "create_time");
            Map(x => x.UpdateTime, "update_time");
            Map(x => x.PayerEmail, "payer_paypal_email");
            Map(x => x.PayerId, "payer_id");
            Map(x => x.Cart, "cart");
        }
    }
}