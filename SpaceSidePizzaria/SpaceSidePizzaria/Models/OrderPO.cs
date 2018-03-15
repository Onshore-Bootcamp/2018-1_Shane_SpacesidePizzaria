using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SpaceSidePizzaria.Models
{
    public class OrderPO
    {
        public long OrderID { get; set; }

        public long UserID { get; set; }

        public List<SelectListItem> StatusSelectListItemsPickup {get; set;}

        public List<SelectListItem> StatusSelectListItemsDelivery { get; set; }
        
        public string Status { get; set; }

        [Required]
        public bool IsDelivery { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? OrderFulfilledTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OrderDate { get; set; }

        public long? DriverID { get; set; }

        public string DriverFirstName { get; set; }

        public string BuyerName { get; set; }

        public string BuyerAddress { get; set; }

        public bool Paid { get; internal set; }

        public decimal Total { get; internal set; }
    }
}