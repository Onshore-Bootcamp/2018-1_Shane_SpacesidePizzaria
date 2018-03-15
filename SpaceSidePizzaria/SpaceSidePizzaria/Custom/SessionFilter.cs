using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceSidePizzaria.Custom
{
    public class SessionRoleFilter : ActionFilterAttribute
    {
        private readonly string _key = String.Empty;
        private readonly int[] _roles;

        /// <summary>
        /// Filters users based on their RoleID.
        /// </summary>
        /// <param name="key">The name of the Key to fetch the RoleID from Session.</param>
        /// <param name="roles">The roles that are allowed for an action.</param>
        public SessionRoleFilter(string key, params int[] roles)
        {
            _key = key;
            _roles = roles ?? new int[0];
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Get the Session from the HttpContext.
            HttpSessionStateBase session = filterContext.HttpContext.Session;

            // If the user is not allowed to go to the action.
            if (session[_key] == null || !int.TryParse(session[_key].ToString(), out int role) || !_roles.Contains(role))
            {
                // Redirect the user to the Login page.
                filterContext.Result = new RedirectResult("/Account/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}