using SpaceSidePizzariaBLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSidePizzariaBLL
{
    public class PizzaBLO
    {
        /// <summary>
        /// Gets the cost of a pizza without tax.
        /// </summary>
        /// <param name="pizzaBO">The pizza to calculate on.</param>
        /// <returns>Return the cost of the pizza.</returns>
        public decimal GetPizzaCost(PizzaBO pizzaBO)
        {
            return PriceCalculator.CalculateBasePizzaCost(pizzaBO);
        }

        /// <summary>
        /// Calculates the total for a given list of pizzas. Returns the total with
        /// tax added.
        /// </summary>
        public decimal GetCostOfPizzas(List<PizzaBO> pizzaBOList)
        {
            return PriceCalculator.CalculateCostOfPizzas(pizzaBOList);
        }

        /// <summary>
        /// Ranks the topping based on how many times they occur in the Db.
        /// </summary>
        /// <param name="allPizzas">All the pizzas in the Db.</param>
        /// <param name="allToppings">All of the available toppings.</param>
        /// <returns>A dictionary with the toppings as keys and their corresponding occurances as the values.</returns>
        public Dictionary<string, int> GetToppingsStats(List<PizzaBO> allPizzas, List<string> allToppings)
        {
            // The return variable.
            // The key will be the topping name, the value will be the amount sold.
            Dictionary<string, int> toppingsSold = new Dictionary<string, int>();

            try
            {
                allPizzas
                    .Select(pizzaBO => pizzaBO.Toppings.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    .SelectMany(toppingsList => toppingsList) // Flatten the toppingsList to a list of strings
                    .GroupBy(topping => topping)
                    .OrderByDescending(group => group.Count())
                    .ToList()
                    .ForEach(group => toppingsSold.Add(group.Key, group.Count()));

                // Go through each topping option that is available to the customer and if the topping
                // has not been used yet, then add them to the dictionary with a value of 0.
                foreach (string topping in allToppings)
                {
                    if (!toppingsSold.Keys.Contains(topping))
                    {
                        toppingsSold.Add(topping, 0);
                    }
                    else { }
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);

                /* I don't think it would be wise to throw an exception here, since 
                 * the calculations are usually ran one after the other. So throwing
                 * and exception here would stop the program from getting any more
                 * stats. 
                 */

                // throw exception;
            }
            finally { }

            return toppingsSold;
        }
    }
}
