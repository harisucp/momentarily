using Apeek.Entities.Entities;
using Apeek.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Mappings
{
  public class PaypalInfoPaymentDetailMap: AuditEntityMap<PaypalInfoPaymentDetail>
    {
        public PaypalInfoPaymentDetailMap()
        {
            Table("p_paypal_info_payment_detail");            Id(x => x.Id, "id");            Map(x => x.UserId, "user_id");            Map(x => x.AccountNumber, "accountnumber");            Map(x => x.RoutingNumber, "routingnumber");            Map(x => x.Locality, "locality");            Map(x => x.PostalCode, "postalCode");            Map(x => x.Region, "region");            Map(x => x.StreetAddress, "streetaddress");            Map(x => x.PaypalBusinessEmail, "paypalbusinessemail");            Map(x => x.PhoneNumber, "phonenumber");            Map(x => x.PaymentType, "paymenttype");
        }
    }
}
