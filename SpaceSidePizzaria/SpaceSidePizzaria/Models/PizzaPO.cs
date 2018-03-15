using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SpaceSidePizzaria.Models
{
    public class PizzaPO
    {
        public long PizzaID { get; set; }

        public long? OrderID { get; set; }

        public List<SelectListItem> CrustSelectListItems { get; set; }
        
        [Required]
        public string Crust { get; set; }

        public List<SelectListItem> SizeSelectListItems { get; set; }

        [Required]
        public byte Size { get; set; }

        public List<SelectListItem> ToppingsSelectListItems { get; set; }

        public string[] ToppingsList { get; set; }

        public string Toppings
        {
            get
            {
                return ToppingsList == null ? null : string.Join(", ", ToppingsList);
            }
            set
            {
                ToppingsList = value.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public List<SelectListItem> SauceSelectListItems { get; set; }

        public string Sauce { get; set; }

        [Required]
        [DisplayName("Add Cheese?")]
        public bool Cheese { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString= "{0:c}")]
        public decimal Price { get; set; }

        [DisplayName("Image")]
        public string ImagePath { get; set; }

        private string GenerateDesciption()
        {
            string description = Size + " Inch pizza " + Crust + " ";

            if (Toppings != null && ToppingsList.Count() > 0)
            {
                description += "with toppings: ";

                description += String.Join(", ", ToppingsList);
            }

            if (Cheese)
            {
                description += " with cheese";
            }
            else
            {
                description += " without cheese";
            }

            description += " and " + Sauce + " sauce.";

            return description;
        }

        private string _desciption;

        [StringLength(140)]
        [Display()]
        public string Description
        {
            get
            {
                string pizzaDescription = _desciption;
                if (String.IsNullOrEmpty(_desciption))
                {
                    pizzaDescription = GenerateDesciption();
                }

                return pizzaDescription;
            }

            set
            {
                _desciption = value;
            }
        }
    }
}