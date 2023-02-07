﻿using Apeek.Entities.Entities;
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
            Table("p_paypal_info_payment_detail");
        }
    }
}