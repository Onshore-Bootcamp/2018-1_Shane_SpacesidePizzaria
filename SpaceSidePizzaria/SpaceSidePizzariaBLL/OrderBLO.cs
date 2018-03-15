using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceSidePizzariaBLL.Models;
using System.Globalization;

namespace SpaceSidePizzariaBLL
{
    public class OrderBLO
    {
        /// <summary>
        /// Finds the user with the most orders.
        /// </summary>
        /// <param name="allOrders">The orders to operate on.</param>
        /// <returns>The primary key of the user with the most orders.</returns>
        public long GetMostValuableCustomer(List<OrderBO> allOrders)
        {
            long customerID = -1;

            try
            {
                customerID = allOrders
                    .GroupBy(orderBO => orderBO.UserID)          // Group by the user's ID.
                    .OrderByDescending(group => group.Count())   // Order by the occurrence.
                    .Select(group => group.Key)
                    .First();
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                // Don't throw the exception.
            }
            finally { }

            return customerID;
        }

        /// <summary>
        /// Provides a method to recieve the differences between delivery and pickup orders for past
        /// months in the current year.
        /// </summary>
        /// <param name="allOrders">Every order from the DB you wish to preform the method on.</param>
        /// <returns>
        ///     A an outer dictionary with keys for each month in the current year the company has orders
        ///     for. The inner dictionary contains the number of "Deliveries" and the number of "Pickups"
        ///     as keys.
        /// </returns>
        public Dictionary<string, Dictionary<string, int>> GetDeliveriesVsPickupsByMonth(List<OrderBO> allOrders)
        {
            // The DateTimeFormatInfo provides a method to get a month name by an int
            DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();

            // Instantiate the return value.
            /* The outer Dictionary keys will be a month's name, the inner Dictionary's keys
             * will be "Deliveries" and "Pickups". */
            Dictionary<string, Dictionary<string, int>> pickupDeliveriesDict = new Dictionary<string, Dictionary<string, int>>();

            try
            {
                // Group the orders by Month but for only the current year and order 
                // the groups by the month name.
                IEnumerable<IGrouping<int, OrderBO>> monthGroups = allOrders
                    .Where
                        (userBO => userBO.OrderFulfilledTime != null &&
                        ((DateTime)userBO.OrderFulfilledTime).Year == DateTime.Now.Year)
                    .GroupBy(orderBO => ((DateTime)orderBO.OrderFulfilledTime).Month)
                    .OrderBy(group => group.Key);

                // Foreach group, count the number of deliveries and pickups and add them to 
                // a dictionary, then add that dictionary to the return Dictionary.
                foreach (IGrouping<int, OrderBO> monthGroup in monthGroups)
                {
                    int deliveries = monthGroup.Count(orderBO => orderBO.IsDelivery == true),
                        pickups = monthGroup.Count() - deliveries;

                    Dictionary<string, int> orderMethod = new Dictionary<string, int>();
                    orderMethod.Add("Deliveries", deliveries);
                    orderMethod.Add("Pickups", pickups);

                    // Add the deliveries and pickup dictionary to our return dictionary.
                    pickupDeliveriesDict.Add(dateTimeFormat.GetMonthName(monthGroup.Key), orderMethod);
                }
            }
            catch (Exception exception)
            {
                // Log the execption, but don't throw it.
                Logger.LogExceptionNoRepeats(exception);

                // throw exception;
            }
            finally { }

            // Finally return the Dictionary.
            return pickupDeliveriesDict;
        }

        /// <summary>
        /// Gets the top 3 drivers and how many orders they've taken.
        /// </summary>
        /// <param name="allOrders">The orders to operate on.</param>
        /// <returns>
        ///     A Dictionary where the keys represent the driver's ID and the
        ///     corresponding values represent the number of orders they've taken.
        /// </returns>
        public Dictionary<long, int> GetMostValuableDrivers(List<OrderBO> allOrders)
        {
            Dictionary<long, int> driverStats = new Dictionary<long, int>();

            try
            {
                allOrders
                .Where(orderBO => orderBO.DriverID != null)  // Select only the orders where there is a driverID.
                .GroupBy(orderBO => orderBO.DriverID)        // Group by the the Drivers ID.
                .OrderByDescending(x => x.Count())           // Order the results by occurrence.
                .Take(3)                                     // Only take the first 3.
                .ToList()
                .ForEach(group => driverStats.Add((long)group.Key, group.Count())); // Add the results to the dictionary.
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                // Don't throw the execption
            }
            finally { }

            return driverStats;
        }
    }
}
