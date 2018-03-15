using SpaceSidePizzariaBLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpaceSidePizzariaBLL
{
    internal static class PriceCalculator
    {
        private const decimal _salesTax = 0.04225M;

        /// <summary>
        /// Calculates the cost of a pizza, returns the base cost (before taxes).
        /// </summary>
        public static decimal CalculateBasePizzaCost(PizzaBO pizza)
        {
            decimal total = pizza.Size - 1;

            if (pizza.Toppings != null)
            {
                int numToppings = pizza.Toppings.Split(',').Count();

                if (numToppings > 2)
                {
                    for (var i = 2; i < numToppings; i++)
                    {
                        total += 1.25M;
                    }
                }
            }

            return total;
        }

        public static decimal CalculateCostOfPizzas(List<PizzaBO> pizzaBOList)
        {
            decimal total = 0;

            foreach (PizzaBO pizzaBO in pizzaBOList)
            {
                total += pizzaBO.Price;
            }

            return total * (1 + _salesTax);
        }

        /// <summary>
        /// Calculates the taxes on a given amount.
        /// </summary>
        public static decimal CalculateTaxes(decimal beforeTax)
        {
            return beforeTax * _salesTax;
        }
    }
}