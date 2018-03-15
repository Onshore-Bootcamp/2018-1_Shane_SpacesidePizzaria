using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SpaceSidePizzaria.Models
{
    public class PaymentPO
    {
        [ReadOnly(true)]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Total { get; set; }

        [DisplayName("For Delivery")]
        public bool ForDelivery { get; set; }

        [DisplayName("Pay With Cash")]
        public bool PayWithCash { get; set; }

        [CreditCard]
        [DataType(DataType.CreditCard)]
        public string CreditCard { get; set; }
    }
}