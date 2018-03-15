using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSidePizzariaBLL.Models
{
    public class PizzaBO
    {
        public long PizzaID { get; set; }

        public long? OrderID { get; set; }

        public string Crust { get; set; }

        public byte Size { get; set; }

        public string Toppings { get; set; }

        public string Sauce { get; set; }

        public bool Cheese { get; set; }

        public decimal Price { get; set; }

        public string ImagePath { get; set; }

        public string Description { get; set; }
    }
}
