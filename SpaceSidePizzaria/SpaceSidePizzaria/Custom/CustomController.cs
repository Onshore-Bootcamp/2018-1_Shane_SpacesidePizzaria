using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpaceSidePizzaria.Custom;
using System.Web.Mvc;
using SpaceSidePizzaria.Models;

namespace SpaceSidePizzaria.Custom
{
    [CartCheckerFilter()]
    public class CustomController : Controller
    {
        /******** Helper Methods ********/

        /// <summary>
        /// Gets the user's RoleID from the Session. Returns the
        /// value of the Key "Role" from Session if it exists otherwise, returns 0.
        /// </summary>
        protected int GetSessionRole()
        {
            int userRole = 0;
            if (Session["Role"] != null)
            {
                int.TryParse(Session["Role"].ToString(), out userRole);
            }

            return userRole;
        }

        /// <summary>
        /// Sets the Keys: Username, Role, and UserID to their corresponding values from the
        /// UserPO that is passed in as a parameter.
        /// </summary>
        /// <remarks>
        /// SetUserSession also logs a message when a Admin logs into their account.
        /// </remarks>
        protected void SetUserSession(UserPO user)
        {
            Session["Username"] = user.Username;
            Session["Role"] = user.RoleID;
            Session["UserID"] = user.UserID;

            if (user.RoleID == 1 || user.RoleID == 2)
            {
                // If an Admin or Driver is logging in then 
                // allow 4 hours before the session times out.
                // One less complaint we need to worry about.
                Session.Timeout = 240;
            }
            else
            {
                // Give users a session timeout of 45.
                Session.Timeout = 45;
            }

            if (user.RoleID == 1)
            {
                // If an Admin got on, then we should log it.
                Logger.Log("Info", "Mvc Layer", "SetUserSession from AccountController",
                    "ADMIN logged on with username " + user.Username);
            }
        }

        /// <summary>
        /// Returns the Sessions UserID if there is one otherwise, 0.
        /// </summary>
        protected long GetSessionUserID()
        {
            int userID = 0;

            if (Session["UserID"] != null)
            {
                int.TryParse(Session["UserID"].ToString(), out userID);
            }

            return userID;
        }

        /// <summary>
        /// Creates a redirect that displays the message from ViewBag.Message and redirects
        /// the User to the specified URL after 3 seconds.
        /// </summary>
        protected ActionResult RedirectingPage(string message, string url)
        {
            ViewBag.Message = message;
            Response.StatusCode = 302; // Redirecting code.
            Response.AppendHeader("Refresh", "3; url=" + url);

            return PartialView("~/Views/Shared/Redirecting.cshtml");
        }

        /// <summary>
        /// Gets a list of invalid information on a user when they request a delivery order.
        /// </summary>
        /// <param name="user">The user to validate.</param>
        /// <returns>A list of property names that are invalid.</returns>
        protected List<string> GetInvalidDeliveryInfo(UserPO user)
        {
            // Set up the return variable. A list of the property names that are invalid.
            List<string> invalidInfo = new List<string>();

            // Set up a dictionary with the Keys as the models properties and the values being true if valid.
            Dictionary<string, bool> validations = new Dictionary<string, bool>();
            validations.Add("Address", !String.IsNullOrEmpty(user.Address));
            validations.Add("City", !String.IsNullOrEmpty(user.City));
            validations.Add("State", !String.IsNullOrEmpty(user.State));
            validations.Add("ZipCode", !String.IsNullOrEmpty(user.ZipCode));
            validations.Add("Phone", !String.IsNullOrEmpty(user.Phone));

            // Loop through all of the keys and add any that are not valid to the list of invalidInfo.
            foreach (string key in validations.Keys)
            {
                if (validations[key] == false)
                {
                    invalidInfo.Add(key);
                }
            }

            return invalidInfo;
        }

        /********************************/
    }
}