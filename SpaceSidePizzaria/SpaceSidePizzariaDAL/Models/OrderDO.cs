using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSidePizzariaDAL.Models
{
    public class OrderDO
    {
        public long OrderID { get; set; }

        public long UserID { get; set; }

        public string Status { get; set; }

        public bool IsDelivery { get; set; }

        public DateTime? OrderFulfilledTime { get; set; }

        public DateTime OrderDate { get; set; }

        public long? DriverID { get; set; }

        public bool Paid { get; set; }

        public decimal Total { get; set; }

        public string DriverFirstName { get; set; }

        public string BuyerName { get; set; }

        public string BuyerAddress { get; set; }
    }
}
