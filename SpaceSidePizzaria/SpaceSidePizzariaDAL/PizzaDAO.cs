using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using SpaceSidePizzariaDAL.Models;
using System.Text;
using System.Threading.Tasks;
using SpaceSidePizzariaDAL.Mapping;

namespace SpaceSidePizzariaDAL
{
    public class PizzaDAO
    {
        private readonly string _dataSource;

        public PizzaDAO(string connectionString)
        {
            _dataSource = connectionString;
        }

        /// <summary>
        /// Attempts to add a pizza to the database. Return a bool,
        /// true if successfull otherwise false.
        /// </summary>
        public bool AddNewPizza(PizzaDO newPizza)
        {
            bool isSuccess = false;  // This will be are return, for whether or not the 
                                     // pizza was added to the database.

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Instantiate the sqlConnection.
                sqlConnection = new SqlConnection(_dataSource);

                // Instantiate our SQL command with our Stored Procedure name.
                sqlCommand = new SqlCommand("CREATE_NEW_PIZZA", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add all of the parameters that the Stored Procedure needs.
                sqlCommand.Parameters.AddWithValue("@OrderID", (object)newPizza.OrderID ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Crust", newPizza.Crust);
                sqlCommand.Parameters.AddWithValue("@Size", newPizza.Size);
                sqlCommand.Parameters.AddWithValue("@Toppings", (object)newPizza.Toppings ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Sauce", (object)newPizza.Sauce ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Cheese", newPizza.Cheese);
                sqlCommand.Parameters.AddWithValue("@Price", newPizza.Price);
                sqlCommand.Parameters.AddWithValue("@ImagePath", (object)newPizza.ImagePath ?? DBNull.Value);

                sqlConnection.Open();

                // Check if the Stored Procedure was successfull or not.
                isSuccess = sqlCommand.ExecuteNonQuery() == 1;

                // Log that the Stored Procedure was unable to add a new Pizza.
                if (!isSuccess)
                {
                    Logger.Log("Warning", "Pizza DAO", "AddNewPizza", "Unable to add a new pizza to the database.");
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Manually dispose of any connections or anything that might be using up resources.
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
            }

            // Return whether or not the command was successfull.
            return isSuccess;
        }

        /// <summary>
        /// Attempts  to add a Admin created pizza to the database.
        /// </summary>
        public bool AddNewPrefabPizza(PizzaDO newPrefabPizza)
        {
            bool success = false;

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("CREATE_NEW_PREFAB_PIZZA", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Crust", newPrefabPizza.Crust);
                sqlCommand.Parameters.AddWithValue("@Size", newPrefabPizza.Size);
                sqlCommand.Parameters.AddWithValue("@Toppings", (object)newPrefabPizza.Toppings ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Sauce", (object)newPrefabPizza.Sauce ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Cheese", newPrefabPizza.Cheese);
                sqlCommand.Parameters.AddWithValue("@Price", newPrefabPizza.Price);
                sqlCommand.Parameters.AddWithValue("@ImagePath", newPrefabPizza.ImagePath);
                sqlCommand.Parameters.AddWithValue("@Description", (object)newPrefabPizza.Description ?? DBNull.Value);

                sqlConnection.Open();

                success = sqlCommand.ExecuteNonQuery() > 0;

                if (!success)
                {
                    Logger.Log("WARNING", "PizzaDAO", "AddNewPrefabPizza",
                        "Unable to add a new prefab pizza to the database.");
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
            }

            return success;
        }

        public List<PizzaDO> GetAllPrefabPizzas()
        {
            List<PizzaDO> prefabPizzas = new List<PizzaDO>();

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("OBTAIN_ALL_PREFAB_PIZZAS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                adapter = new SqlDataAdapter(sqlCommand);
                DataTable pizzaTable = new DataTable();

                adapter.Fill(pizzaTable);

                if (pizzaTable.Rows.Count == 0)
                {
                    Logger.Log("WARNING", "PizzaDAO", "GetAllPrefabPizzas",
                        "There seem to be no prefab pizzas in the database.");
                }
                else
                {
                    prefabPizzas = Mapping.PizzaDataTableMapper.DataTableToPizzaDOList(pizzaTable);
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();
                }
            }

            return prefabPizzas;
        }

        public List<PizzaDO> GetAllPizzas()
        {
            List<PizzaDO> prefabPizzas = new List<PizzaDO>();

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("OBTAIN_ALL_PIZZAS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                adapter = new SqlDataAdapter(sqlCommand);
                DataTable pizzaTable = new DataTable();

                adapter.Fill(pizzaTable);

                if (pizzaTable.Rows.Count == 0)
                {
                    Logger.Log("WARNING", "PizzaDAO", "GetAllPizzas",
                        "There seem to be no pizzas in the database.");
                }
                else
                {
                    prefabPizzas = Mapping.PizzaDataTableMapper.DataTableToPizzaDOList(pizzaTable);
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();
                }
            }

            return prefabPizzas;
        }

        /// <summary>
        /// Attempts to find a Pizza by an ID. Returns a filled out PizzaDO if
        /// the Stored Procedure was able to find a Pizza by the parameter or
        /// false if the was no Pizza in the database with that ID.
        /// </summary>
        public PizzaDO ViewPizzaByID(long pizzaID)
        {
            PizzaDO pizzaDO = null;

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Instantiate the sqlConnection
                sqlConnection = new SqlConnection(_dataSource);

                // Instantiate our SQL command with our Stored Procedure name.
                sqlCommand = new SqlCommand("OBTAIN_PIZZA_BY_ID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add the PizzaID parameter for the Stored Procedure.
                sqlCommand.Parameters.AddWithValue("@PizzaID", pizzaID);

                // Instantiate the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Create a new Table to hold the information we recieve from the database.
                DataTable pizzaTable = new DataTable();

                sqlConnection.Open();

                // Fill our DataTable with the information we recieved.
                adapter.Fill(pizzaTable);

                if (pizzaTable.Rows.Count > 0)
                {
                    // The sqlCommand should only find one pizza by the pizzaID,
                    // If for some reason the table has more than 1 row, then
                    // log the pizza ID.
                    if (pizzaTable.Rows.Count > 1)
                    {
                        Logger.Log("WARNING", "PizzaDAO", "ViewPizzaByID",
                            "Two pizzas have the same ID number. Is the PizzaID a primary key? " +
                            "PizzaID: " + pizzaID);
                    }

                    pizzaDO = PizzaDataTableMapper.DataRowToPizzaDO(pizzaTable.Rows[0]);
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw;
            }
            finally
            {
                // Manually dispose of any connections or anything that might be using up resources.
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();
                }
            }

            return pizzaDO;
        }

        /// <summary>
        /// Attempts to update a Pizza in the Pizza Table.
        /// </summary>
        public int UpdatePizza(PizzaDO updatedPizza)
        {
            int rowsAffected = 0;

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Instantiate the sqlConnection.
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("UPDATE_PIZZA", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@PizzaID", updatedPizza.PizzaID);
                sqlCommand.Parameters.AddWithValue("@Crust", updatedPizza.Crust);
                sqlCommand.Parameters.AddWithValue("@Size", updatedPizza.Size);
                sqlCommand.Parameters.AddWithValue("@Toppings", (object)updatedPizza.Toppings ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Sauce", (object)updatedPizza.Sauce ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Cheese", updatedPizza.Cheese);
                sqlCommand.Parameters.AddWithValue("@Price", updatedPizza.Price);
                sqlCommand.Parameters.AddWithValue("@ImagePath", (object)updatedPizza.ImagePath ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Description", (object)updatedPizza.Description ?? DBNull.Value);

                sqlConnection.Open();

                // Capture the number of rows affected after executing the update stored procedure.
                rowsAffected = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                // If the Stored procedure updated the Pizza table, then log some information about it.
                if (rowsAffected > 0)
                {
                    Logger.Log("INFO", "PizzaDAO", "UpdatePizza",
                        "Updated PizzaID " + updatedPizza.PizzaID + "  Rows Affected: " + rowsAffected);
                }
            }

            // Return the number of rows affected.
            return rowsAffected;
        }

        /// <summary>
        /// Attempts to delete a pizza form the database. Return the number
        /// of rows affected.
        /// </summary>
        public int DeletePizza(long pizzaID)
        {
            int rowsAffected = 0;

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Initialize the connection to the SQL database.
                sqlConnection = new SqlConnection(_dataSource);

                // Instantiate a new Stored Procedure SQL command.
                sqlCommand = new SqlCommand("DELETE_PIZZA_BY_ID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add the PizzaID to the parameters of the sqlCommand.
                sqlCommand.Parameters.AddWithValue("@PizzaID", pizzaID);

                sqlConnection.Open();

                // Capture the number of rows affected.
                rowsAffected = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }

                // If anything was deleted then log the PizzaID and the number of rows affected.
                if (rowsAffected > 0)
                {
                    Logger.Log("INFO", "PizzaDAO", "DeletePizza",
                        "Deleted pizza with ID: " + pizzaID + "  Number of rows affected" + rowsAffected);
                }
            }

            return rowsAffected;
        }

        public List<PizzaDO> GetPizzasByOrderID(long orderID)
        {
            List<PizzaDO> pizzaPOList = new List<PizzaDO>();

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("OBTAIN_PIZZAS_BY_ORDERID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@OrderID", orderID);

                adapter = new SqlDataAdapter(sqlCommand);
                DataTable pizzaTable = new DataTable();

                adapter.Fill(pizzaTable);

                pizzaPOList = Mapping.PizzaDataTableMapper.DataTableToPizzaDOList(pizzaTable);
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();
                }
            }

            return pizzaPOList;
        }
    }
}
