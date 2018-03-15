using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SpaceSidePizzariaDAL.Models;

namespace SpaceSidePizzariaDAL.Mapping
{
    public static class UserDataTableMapper
    {
        public static UserDO UserRowToUserDO(DataRow userRow)
        {
            UserDO userDO = new UserDO();

            try
            {
                userDO.UserID = long.Parse(userRow["UserID"].ToString());
                userDO.Username = userRow["Username"].ToString();
                userDO.Password = userRow["Password"].ToString();
                userDO.Email = userRow["Email"].ToString();
                userDO.FirstName = userRow["FirstName"].ToString();
                userDO.LastName = userRow["LastName"].ToString();
                userDO.RoleID = byte.Parse(userRow["RoleID"].ToString());
                userDO.DateAdded = DateTime.Parse(userRow["DateAdded"].ToString());

                // Allowed to be Null in DB
                userDO.ZipCode = userRow["ZipCode"].ToString();
                userDO.Phone = userRow["Phone"].ToString();
                userDO.Address = userRow["Address"].ToString();
                userDO.City = userRow["City"].ToString();
                userDO.State = userRow["State"].ToString();
            }
            catch (Exception exception)
            {
                // If there was an exception thrown in the try block then
                // set userDO to a Null pointer, so we can check for null
                // in the calling method.
                userDO = null;

                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // TODO: Find something to put in the finally.
            }

            return userDO;
        }

        public static List<UserDO> UserTableToUserList(DataTable userTable)
        {
            List<UserDO> userList = new List<UserDO>();

            foreach (DataRow row in userTable.Rows)
            {
                userList.Add(UserRowToUserDO(row));
            }

            return userList;
        }
    }
}