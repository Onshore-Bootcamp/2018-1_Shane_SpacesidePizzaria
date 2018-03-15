using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpaceSidePizzaria.Models;

namespace SpaceSidePizzaria.ViewModels
{
    public class PizzaOrderVM
    {
        public List<PizzaPO> Pizzas { get; set; }
        public OrderPO Order { get; set; }
    }
}