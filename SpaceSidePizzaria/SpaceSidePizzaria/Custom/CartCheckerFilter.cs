using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpaceSidePizzaria.Models;

namespace SpaceSidePizzaria.Custom
{
    /// <summary>
    /// Creates a cart in the Session if one doesn't already exist.
    /// </summary>
    public class CartCheckerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // If there is no Cart key in Session, then add one.
            if (filterContext.HttpContext.Session["Cart"] == null)
            {
                filterContext.HttpContext.Session["Cart"] = new List<PizzaPO>();
            }

            base.OnActionExecuting(filterContext);
        }
    }
}