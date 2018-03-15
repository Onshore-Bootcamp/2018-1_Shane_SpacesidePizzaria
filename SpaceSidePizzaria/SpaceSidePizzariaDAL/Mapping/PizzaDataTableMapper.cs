using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SpaceSidePizzariaDAL.Models;

namespace SpaceSidePizzariaDAL.Mapping
{
    public static class PizzaDataTableMapper
    {
        public static PizzaDO DataRowToPizzaDO (DataRow row)
        {
            PizzaDO pizzaDO = new PizzaDO();

            try
            {
                pizzaDO.OrderID = row["OrderID"] as long?;
                pizzaDO.PizzaID = long.Parse(row["PizzaID"].ToString());
                pizzaDO.Crust = row["Crust"].ToString();
                pizzaDO.Size = byte.Parse(row["Size"].ToString());
                pizzaDO.Toppings = row["Toppings"].ToString();
                pizzaDO.Sauce = row["Sauce"].ToString();
                pizzaDO.Cheese = bool.Parse(row["Cheese"].ToString());
                pizzaDO.Price = decimal.Parse(row["Price"].ToString());
                pizzaDO.ImagePath = row["ImagePath"].ToString();
                pizzaDO.Description = row["Description"].ToString();
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally { }

            return pizzaDO;
        }

        public static List<PizzaDO> DataTableToPizzaDOList (DataTable pizzaTable)
        {
            List<PizzaDO> pizzaList = new List<PizzaDO>();

            foreach (DataRow row in pizzaTable.Rows)
            {
                pizzaList.Add(DataRowToPizzaDO(row));
            }

            return pizzaList;
        }
    }
}
