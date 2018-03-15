using System;
using System.Collections.Generic;
using SpaceSidePizzaria.Models;
using SpaceSidePizzariaDAL.Models;
using SpaceSidePizzariaBLL.Models;
using System.Linq;
using System.Web;

namespace SpaceSidePizzaria.Mapping
{
    public static class PizzaMapper
    {
        public static PizzaPO PizzaDOtoPizzaPO(PizzaDO from)
        {
            PizzaPO to = new PizzaPO();

            to.PizzaID = from.PizzaID;
            to.OrderID = from.OrderID;
            to.Cheese = from.Cheese;
            to.Crust = from.Crust;
            to.ImagePath = from.ImagePath;
            to.Price = from.Price;
            to.Sauce = from.Sauce;
            to.Size = from.Size;
            to.Toppings = from.Toppings;
            to.Description = from.Description;

            return to;
        }

        public static PizzaDO PizzaPOtoPizzaDO(PizzaPO from)
        {
            PizzaDO to = new PizzaDO();

            to.PizzaID = from.PizzaID;
            to.OrderID = from.OrderID;
            to.Cheese = from.Cheese;
            to.Crust = from.Crust;
            to.ImagePath = from.ImagePath;
            to.Price = from.Price;
            to.Sauce = from.Sauce;
            to.Size = from.Size;
            to.Toppings = from.Toppings;
            to.Description = from.Description;

            return to;
        }

        public static PizzaBO PizzaPOtoPizzaBO(PizzaPO from)
        {
            PizzaBO to = new PizzaBO();

            to.PizzaID = from.PizzaID;
            to.OrderID = from.OrderID;
            to.Cheese = from.Cheese;
            to.Crust = from.Crust;
            to.ImagePath = from.ImagePath;
            to.Price = from.Price;
            to.Sauce = from.Sauce;
            to.Size = from.Size;
            to.Toppings = from.Toppings;
            to.Description = from.Description;

            return to;
        }

        public static PizzaBO PizzaDOtoPizzaBO(PizzaDO from)
        {
            PizzaBO to = new PizzaBO();

            to.PizzaID = from.PizzaID;
            to.OrderID = from.OrderID;
            to.Cheese = from.Cheese;
            to.Crust = from.Crust;
            to.ImagePath = from.ImagePath;
            to.Price = from.Price;
            to.Sauce = from.Sauce;
            to.Size = from.Size;
            to.Toppings = from.Toppings;
            to.Description = from.Description;

            return to;
        }

        public static List<PizzaBO> PizzaDOListToPizzaBOList(List<PizzaDO> pizzaDOList)
        {
            return pizzaDOList.Select(pizzaDO => PizzaDOtoPizzaBO(pizzaDO)).ToList();
        }

        public static List<PizzaBO> PizzaPOListToPizzaBOList(List<PizzaPO> pizzaPOList)
        {
            return pizzaPOList.Select(pizzaPO => PizzaPOtoPizzaBO(pizzaPO)).ToList();
        }

        public static List<PizzaPO> PizzaDOListToPizzaPOList(List<PizzaDO> pizzaDOList)
        {
            return pizzaDOList.Select(pizzaDO => PizzaDOtoPizzaPO(pizzaDO)).ToList();
        }

    }
}