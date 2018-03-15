using System;
using SpaceSidePizzaria.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SpaceSidePizzaria.ViewModels
{
    public class CartPaymentVM
    {
        public List<PizzaPO> Cart { get; set; }

        public PaymentPO PaymentPO { get; set; }

        public CartPaymentVM() { }

        public CartPaymentVM(List<PizzaPO> cart, PaymentPO paymentPO)
        {
            Cart = cart;
            PaymentPO = paymentPO;
        }
    }
}