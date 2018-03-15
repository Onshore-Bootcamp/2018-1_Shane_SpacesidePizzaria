using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SpaceSidePizzariaDAL.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceSidePizzariaDAL.Models;

namespace SpaceSidePizzariaDAL
{
    public class OrderDAO
    {
        private readonly string _connectionString;

        public OrderDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Inserts a new order in the DB and returns the primary key of the newly
        /// created order.
        /// </summary>
        /// <param name="newOrder">The object whose properties will be stored in the DB.</param>
        /// <returns>The Scope Indentity of the newly inserted Order.</returns>
        public long CreateOrder(OrderDO newOrder)
        {
            // Set up the return value.
            long incrementedID = -1;

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Initialize the connection to SQL
                sqlConnection = new SqlConnection(_connectionString);

                // Initialize a the stored procedure
                sqlCommand = new SqlCommand("CREATE_ORDER", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add the paramaters that the stored procedure requires.
                sqlCommand.Parameters.AddWithValue("@UserID", newOrder.UserID);
                sqlCommand.Parameters.AddWithValue("@Status", newOrder.Status);
                sqlCommand.Parameters.AddWithValue("@IsDelivery", newOrder.IsDelivery);
                sqlCommand.Parameters.AddWithValue("@OrderDate", newOrder.OrderDate);
                sqlCommand.Parameters.AddWithValue("@Paid", newOrder.Paid);
                sqlCommand.Parameters.AddWithValue("@Total", newOrder.Total);

                // Open the connection to the database.
                sqlConnection.Open();

                // Get the ID of the newly inserted Order.
                incrementedID =  Convert.ToInt64(sqlCommand.ExecuteScalar());
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Manually close anything that has a dispose.
                if (sqlConnection == null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand == null)
                {
                    sqlCommand.Dispose();
                }
            }

            return incrementedID;
        }
       
        /// <summary>
        /// Gets a OrderDO by an ID.
        /// </summary>
        /// <param name="orderID">The primary key of the order.</param>
        /// <returns>Returns the Order if it was found otherwise returns null.</returns>
        public OrderDO GetOrderByID(long orderID)
        {
            // Start of with the orderDO set to null.
            OrderDO orderDO = null;

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initialize the sqlConnection.
                sqlConnection = new SqlConnection(_connectionString);

                // Initialize the sqlCommand
                sqlCommand = new SqlCommand("OBTAIN_ORDER_BY_ORDERID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add the paramerter this stroed procedure requires.
                sqlCommand.Parameters.AddWithValue("@OrderID", orderID);

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);
                
                // Instantiate a new DataTable to store the results from the stored procedure.
                DataTable orderTable = new DataTable();

                // Open the connection.
                sqlConnection.Open();
                
                // Fill the DataTable with the results from the stored procedure.
                adapter.Fill(orderTable);

                // If the DataTable has any rows..
                if (orderTable.Rows.Count > 0)
                {
                    // Map the row to the orderDO.
                    orderDO = OrderDataTableMapping.DataRowToOrderDO(orderTable.Rows[0]);
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Manually dispose.
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

            return orderDO;
        }

        /// <summary>
        /// Gets every order in the DB.
        /// </summary>
        /// <returns>The a list of OrderDO.</returns>
        public List<OrderDO> GetAllOrders()
        {
            // Instantiate a new list of OrderDOs.
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initialize the sqlConnection.
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_ALL_ORDERS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                
                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Instatiate a new DataTable to hold the results of the stored procedure.
                DataTable orderTable = new DataTable();

                // Open the SQL connection.
                sqlConnection.Open();

                // Fill the DataTable.
                adapter.Fill(orderTable);

                // Map the DataTable to a list of orderDOs.
                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Manually dispose
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

            // Return the mapped results.
            return orderList;
        }

        /// <summary>
        /// Gets any order that has the "status" value passed in to this method.
        /// </summary>
        /// <param name="status">The status to filter by in the DB.</param>
        /// <returns>An filtered OrderDO list.</returns>
        public List<OrderDO> GetOrdersByStatus(string status)
        {
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initialize the sqlConnection.
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_ORDERS_BY_STATUS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add the parameters for the stored procedure.
                sqlCommand.Parameters.AddWithValue("@Status", status);

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Instantiate a new DataData
                DataTable orderTable = new DataTable();

                // Open the connection to SQL.
                sqlConnection.Open();

                // Fill the Data Table with the results from the stored procedure.
                adapter.Fill(orderTable);

                // Mapp the DataTable to a list of OrderDOs
                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Manually close any connections.
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

            return orderList;
        }

        /// <summary>
        /// Gets every delivery order in the DB.
        /// </summary>
        /// <returns>An list of OrderDOs</returns>
        public List<OrderDO> GetDeliveryOrders()
        {
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_ALL_DELIVERY_ORDERS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Create a new DataTable to hold the results of the stored procedure.
                DataTable orderTable = new DataTable();

                // Open the connection to SQL
                sqlConnection.Open();

                // Fill the Data Table with the results from the stored procedure.
                adapter.Fill(orderTable);

                // Map the DataTable rows to a list of OrderDOs.
                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Manually dispose.
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

            return orderList;
        }

        /// <summary>
        /// Gets every delivery order in the DB with the status of "Ready For Delivery".
        /// </summary>
        /// <returns>A list of OrderDOs</returns>
        public List<OrderDO> GetReadyDeliveryOrders()
        {
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_READY_DELIVERY_ORDERS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Create a new DataTable to hold the results of the stored procedure.
                DataTable orderTable = new DataTable();

                sqlConnection.Open();

                // Fill the Data Table with the results from the stored procedure.
                adapter.Fill(orderTable);

                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
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

            return orderList;
        }

        /// <summary>
        /// Gets every delivery order for a specific driver.
        /// </summary>
        /// <param name="driverID">The primary key of a driver.</param>
        /// <returns>A list of OrderDOs</returns>
        public List<OrderDO> GetEnRouteDeliveryOrders(long driverID)
        {
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_ENROUTE_ORDERS_BY_DRIVERID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@DriverID", driverID);

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Create a new DataTable to hold the results of the stored procedure.
                DataTable orderTable = new DataTable();

                sqlConnection.Open();

                // Fill the Data Table with the results from the stored procedure.
                adapter.Fill(orderTable);

                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
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

            return orderList;
        }

        /// <summary>
        /// Gets every order that a user has in the DB.
        /// </summary>
        /// <param name="userID">The primary key of a user to filter by.</param>
        /// <returns>A list of OrderDOs</returns>
        public List<OrderDO> GetOrdersByUserID(long userID)
        {
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_ORDERS_BY_USERID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@UserID", userID);

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Create a new DataTable to hold the results of the stored procedure.
                DataTable orderTable = new DataTable();

                sqlConnection.Open();

                // Fill the Data Table with the results from the stored procedure.
                adapter.Fill(orderTable);

                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
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

            return orderList;
        }

        /// <summary>
        /// Gets every order filtered by the user's ID and a status on an order.
        /// </summary>
        /// <param name="userID">Primary key of a user to filter by.</param>
        /// <param name="status">The status by which to filter by.</param>
        public List<OrderDO> GetOrdersByUserID(long userID, string status)
        {
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_ORDERS_BY_STATUS_AND_USERID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Status", status);
                sqlCommand.Parameters.AddWithValue("@UserID", userID);

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Create a new DataTable to hold the results of the stored procedure.
                DataTable orderTable = new DataTable();

                sqlConnection.Open();

                // Fill the Data Table with the results from the stored procedure.
                adapter.Fill(orderTable);

                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
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

            return orderList;
        }

        /// <summary>
        /// Gets every pending order in the DB.
        /// </summary>
        /// <returns>A list of OrderDOs with any status not matching "Complete".</returns>
        public List<OrderDO> GetPendingOrders()
        {
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_ALL_PENDING_ORDERS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Create a new DataTable to hold the results of the stored procedure.
                DataTable orderTable = new DataTable();

                sqlConnection.Open();

                // Fill the Data Table with the results from the stored procedure.
                adapter.Fill(orderTable);

                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
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

            return orderList;
        }

        /// <summary>
        /// Gets every pending order for in the DB for a specific user.
        /// </summary>
        /// <param name="userID">The primary key of the user by which to filter.</param>
        /// <returns>A list of OrderDOs</returns>
        public List<OrderDO> GetPendingOrders(long userID)
        {
            List<OrderDO> orderList = new List<OrderDO>();

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("OBTAIN_PENDING_ORDERS_BY_USERID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@UserID", userID);

                // Initialize the adapter.
                adapter = new SqlDataAdapter(sqlCommand);

                // Create a new DataTable to hold the results of the stored procedure.
                DataTable orderTable = new DataTable();

                sqlConnection.Open();

                // Fill the Data Table with the results from the stored procedure.
                adapter.Fill(orderTable);

                orderList = OrderDataTableMapping.DataTableToOrderDOs(orderTable);
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

            return orderList;
        }

        /// <summary>
        /// Updates the "DriverID" and and the "Status" columns in the database.
        /// </summary>
        /// <param name="orderID">The primary key of an order to remove from a drivers list of orders.</param>
        /// <returns>A boolean, true if successful otherwise false.</returns>
        public bool RemoveDeliveryFromDriver(long orderID)
        {
            bool success = false;

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("REMOVE_DELIVERY_FROM_DRIVER", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@OrderID", orderID);

                sqlConnection.Open();

                success = sqlCommand.ExecuteNonQuery() > 0;
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection == null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand == null)
                {
                    sqlCommand.Dispose();
                }
            }

            return success;
        }

        /// <summary>
        /// Updates the "Status" column on a order to "Complete".
        /// </summary>
        /// <param name="orderID">The primary key of an order to change the status of.</param>
        public bool CompleteOrder(long orderID)
        {
            bool success = false;

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("COMPLETE_ORDER", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@OrderID", orderID);

                sqlConnection.Open();

                success = sqlCommand.ExecuteNonQuery() > 0;
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection == null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand == null)
                {
                    sqlCommand.Dispose();
                }
            }

            return success;
        }

        /// <summary>
        /// Updates an order's "Total" column to a new value.
        /// </summary>
        /// <param name="orderID">The primary key of an order to filter by.</param>
        /// <param name="newTotal">The new value for the order.</param>
        /// <returns>A boolean, true if successfull otherwise false.</returns>
        public bool UpdateOrderTotal(long orderID, decimal newTotal)
        {
            bool success = false;

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("UPDATE_ORDER_TOTAL_BY_ORDERID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@OrderID", orderID);
                sqlCommand.Parameters.AddWithValue("@Total", newTotal);

                sqlConnection.Open();

                success = sqlCommand.ExecuteNonQuery() == 1;
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                if (sqlConnection == null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand == null)
                {
                    sqlCommand.Dispose();
                }
            }

            return success;
        }

        /// <summary>
        /// Updates an order's status to the supplied status.
        /// </summary>
        /// <param name="orderID">The primary key of an order.</param>
        /// <param name="status">The new value for the order's status.</param>
        public bool UpdateOrderStatus(long orderID, string status)
        {
            bool success = false;

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("UPDATE_ORDER_STATUS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add the parameters that the stored procdure requires.
                sqlCommand.Parameters.AddWithValue("@OrderID", orderID);
                sqlCommand.Parameters.AddWithValue("@Status", status);

                // Open the connection to the SQL Db.
                sqlConnection.Open();
                
                // Determine whether or not the command was successfull.
                success = sqlCommand.ExecuteNonQuery() == 1;
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Dispose
                if (sqlConnection == null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand == null)
                {
                    sqlCommand.Dispose();
                }
            }

            return success;
        }

        /// <summary>
        /// Attempts to delete an order in the DB.
        /// </summary>
        /// <param name="orderID">The primary key of the order to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public int DeleteOrder(long orderID)
        {
            int rowsAffected = 0;

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("DELETE_ORDER_BY_ID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add the parameter to the stored procedure.
                sqlCommand.Parameters.AddWithValue("@OrderID", orderID);

                // Open the connection
                sqlConnection.Open();

                // Capture the number of rows affected after running the command.
                rowsAffected = sqlCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Logger.Log("INFO", "OrderDAO", "DeleteOrder",
                        "Deleted Order #" + orderID + " Rows Affected: " + rowsAffected);
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Dispose
                if (sqlConnection == null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand == null)
                {
                    sqlCommand.Dispose();
                }
            }

            return rowsAffected;
        }

        /// <summary>
        /// Updates an orders DriverID column to a specified driverID.
        /// </summary>
        /// <param name="orderID">The primary key of the order</param>
        /// <param name="driverID">The primary key of the driver.</param>
        public bool AssignDriverToOrder(long orderID, long driverID)
        {
            bool success = false;

            // Declare the variables used to interact with SQL.
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                // Initailize the sqlConnection
                sqlConnection = new SqlConnection(_connectionString);

                // Set sqlCommand to use a defined stored procedure.
                sqlCommand = new SqlCommand("ASSIGN_DRIVER_TO_ORDER", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                // Add the parameters to the command.
                sqlCommand.Parameters.AddWithValue("@OrderID", orderID);
                sqlCommand.Parameters.AddWithValue("@DriverID", driverID);

                // Open the connection to the SQL Db.
                sqlConnection.Open();

                // Determine the success.
                success = sqlCommand.ExecuteNonQuery() == 1;
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // Dispose.
                if (sqlConnection == null)
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (sqlCommand == null)
                {
                    sqlCommand.Dispose();
                }
            }

            return success;
        }
    }
}
