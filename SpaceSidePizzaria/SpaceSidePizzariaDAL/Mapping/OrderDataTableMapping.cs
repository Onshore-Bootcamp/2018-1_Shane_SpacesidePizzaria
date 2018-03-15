using SpaceSidePizzariaDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceSidePizzariaDAL.Mapping
{
    public static class OrderDataTableMapping
    {
        public static OrderDO DataRowToOrderDO(DataRow row)
        {
            OrderDO orderDO = new OrderDO();

            try
            {
                orderDO.OrderID = long.Parse(row["OrderID"].ToString());
                orderDO.UserID = long.Parse(row["UserID"].ToString());
                orderDO.Status = row["Status"].ToString();
                orderDO.IsDelivery = bool.Parse(row["IsDelivery"].ToString());
                orderDO.OrderFulfilledTime = row["OrderFulfilledTime"] as DateTime?;
                orderDO.DriverID = row["DriverID"] as long?;
                orderDO.DriverFirstName = row["DriverFirstName"].ToString();
                orderDO.OrderDate = (DateTime)row["OrderDate"];
                orderDO.BuyerName = row["BuyerName"].ToString();
                orderDO.BuyerAddress = row["BuyerAddress"].ToString();
                orderDO.Total = (decimal)row["Total"];
                orderDO.Paid = (bool)row["Paid"];
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally { }

            return orderDO;
        }

        public static List<OrderDO> DataTableToOrderDOs (DataTable dataTable)
        {
            List<OrderDO> orderList = new List<OrderDO>();

            foreach (DataRow row in dataTable.Rows)
            {
                orderList.Add(DataRowToOrderDO(row));
            }

            return orderList;
        }
    }
}
