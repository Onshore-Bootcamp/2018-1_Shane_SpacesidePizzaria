using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpaceSidePizzaria.Models;
using SpaceSidePizzariaDAL.Models;

namespace SpaceSidePizzaria.Mapping
{
    public static class UserMapper
    {
        /// <summary>
        /// Takes a UserDO model as a parameter and maps it to a UserPO model.
        /// </summary>
        public static UserPO UserDOtoUserPO(UserDO from)
        {
            UserPO to = new UserPO();

            try
            {
                to.UserID = from.UserID;
                to.Username = from.Username;
                to.Password = from.Password;
                to.Email = from.Email;
                to.FirstName = from.FirstName;
                to.LastName = from.LastName;
                to.ZipCode = from.ZipCode;
                to.RoleID = from.RoleID;
                to.Phone = from.Phone;
                to.Address = from.Address;
                to.City = from.City;
                to.State = from.State;
                to.DateAdded = from.DateAdded;
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {
                // TODO: add a finally - maybe something like hashing for the password.
            }


            return to;
        }

        /// <summary>
        /// Takes a UserPO model as a parameter and maps it to a UserDO model.
        /// </summary>
        public static UserDO UserPOtoUserDO(UserPO from)
        {
            UserDO to = new UserDO();

            try
            {
                to.UserID = from.UserID;
                to.Username = from.Username;
                to.Password = from.Password;
                to.Email = from.Email;
                to.FirstName = from.FirstName;
                to.LastName = from.LastName;
                to.ZipCode = from.ZipCode;
                to.RoleID = from.RoleID;
                to.Phone = from.Phone;
                to.Address = from.Address;
                to.City = from.City;
                to.State = from.State;
                to.DateAdded = from.DateAdded;
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
                throw exception;
            }
            finally
            {

            }

            return to;
        }
    }
}