using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceSidePizzariaDAL.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SpaceSidePizzariaDAL.Mapping;

namespace SpaceSidePizzariaDAL
{
    public class UserDAO
    {
        public UserDAO(string connectionString)
        {
            _dataSource = connectionString;
        }

        private readonly string _dataSource;
        
        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// 
        /// <returns>
        /// Returns true if the number of rows affected from the none query is
        /// greater than zero otherwise false.
        /// </returns>
        public bool AddNewUser(UserDO newUser)
        {
            bool succsess = false;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("ADD_NEW_USER", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Username", newUser.Username);
                sqlCommand.Parameters.AddWithValue("@Password", newUser.Password);
                sqlCommand.Parameters.AddWithValue("@Email", newUser.Email);
                sqlCommand.Parameters.AddWithValue("@FirstName", newUser.FirstName);
                sqlCommand.Parameters.AddWithValue("@LastName", newUser.LastName);
                sqlCommand.Parameters.AddWithValue("@ZipCode", (object)newUser.ZipCode ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@RoleID", newUser.RoleID);
                sqlCommand.Parameters.AddWithValue("@Phone", (object)newUser.Phone ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Address", (object)newUser.Address ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@City", (object)newUser.City ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@State", (object)newUser.State ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@DateAdded", newUser.DateAdded);

                sqlConnection.Open();

                succsess = sqlCommand.ExecuteNonQuery() > 0;
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

            return succsess;
        }

        /// <summary>
        /// Finds a User in the database by a username. Returns a UserDO object
        /// if only 1 User by that username exists in the database otherwise
        /// the UserDO will be null.
        /// </summary>
        public UserDO GetUserByUsername(string username)
        {
            UserDO userDO = null;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("OBTAIN_USER_BY_USERNAME", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Username", username);

                adapter = new SqlDataAdapter(sqlCommand);

                DataTable userTable = new DataTable();

                sqlConnection.Open();

                adapter.Fill(userTable);

                if (userTable.Rows.Count == 1)
                {
                    userDO = UserDataTableMapper.UserRowToUserDO(userTable.Rows[0]);
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

            return userDO;
        }

        /// <summary>
        /// Finds a User in the database by a UserID. Returns a UserDO object
        /// if only 1 User by that UserID exists in the database otherwise
        /// the UserDO will be null.
        /// </summary>
        public UserDO GetUserByID(long userID)
        {
            UserDO userDO = null;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("OBTAIN_USER_BY_ID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@UserID", userID);

                adapter = new SqlDataAdapter(sqlCommand);

                DataTable userTable = new DataTable();

                sqlConnection.Open();

                adapter.Fill(userTable);

                if (userTable.Rows.Count == 1)
                {
                    userDO = UserDataTableMapper.UserRowToUserDO(userTable.Rows[0]);
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

            return userDO;
        }
        
        /// <summary>
        /// Updates a User in the database. Returns number of rows affected.
        /// </summary>
        public int UpdateUserByID(UserDO user)
        {
            int rowsAffected = 0;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);
                sqlCommand = new SqlCommand("UPDATE_USER_BY_ID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@UserID", user.UserID);
                sqlCommand.Parameters.AddWithValue("@Username", user.Username);
                sqlCommand.Parameters.AddWithValue("@Email", user.Email);
                sqlCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                sqlCommand.Parameters.AddWithValue("@LastName", user.LastName);
                sqlCommand.Parameters.AddWithValue("@ZipCode", (object)user.ZipCode ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@RoleID", user.RoleID);
                sqlCommand.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@Address", (object)user.Address ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@City", (object)user.City ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@State", (object)user.State ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@DateAdded", user.DateAdded);

                sqlConnection.Open();

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
            }

            return rowsAffected;
        }

        /// <summary>
        /// Tries to delete a user form the database by using an ID.
        /// Returns the number of rows affected.
        /// </summary>
        public int DeleteUserByID(long userID)
        {
            int rowsAffected = 0;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("DELETE_USER_BY_ID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@UserID", userID);

                sqlConnection.Open();

                rowsAffected = sqlCommand.ExecuteNonQuery();

                // We should probably log anytime a user is deleted.
                string message = $"User with {userID} was deleted from the database.  Number of rows affected: {rowsAffected}";
                Logger.Log("Info", "Data Access", "DeleteUserByID", message);
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

            return rowsAffected;
        }

        public int DeleteDriverByID(long userID)
        {
            int rowsAffected = 0;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("DELETE_DRIVER_BY_ID", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@DriverID", userID);

                sqlConnection.Open();

                rowsAffected = sqlCommand.ExecuteNonQuery();

                // We should probably log anytime a driver is deleted.
                string message = $"Driver with {userID} was deleted from the database.  Number of rows affected: {rowsAffected}";
                Logger.Log("Info", "User Data Access", "DeleteDriverByID", message);
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

            return rowsAffected;
        }

        /// <summary>
        /// Attempts to retreive all the Users from the database.
        /// </summary>
        public List<UserDO> GetAllUsers()
        {
            List<UserDO> userDOList = new List<UserDO>();

            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataAdapter adapter = null;

            try
            {
                sqlConnection = new SqlConnection(_dataSource);

                sqlCommand = new SqlCommand("OBTAIN_ALL_USERS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                adapter = new SqlDataAdapter(sqlCommand);
                DataTable userTable = new DataTable();

                sqlConnection.Open();

                adapter.Fill(userTable);

                userDOList = UserDataTableMapper.UserTableToUserList(userTable);
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

            return userDOList;
        }
    }
}
