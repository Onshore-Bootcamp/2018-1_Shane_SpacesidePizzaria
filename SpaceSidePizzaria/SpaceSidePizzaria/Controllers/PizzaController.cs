using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpaceSidePizzariaDAL;
using SpaceSidePizzariaDAL.Models;
using SpaceSidePizzaria.Models;
using System.Configuration;
using System.IO;
using SpaceSidePizzaria.Custom;
using SpaceSidePizzariaBLL;
using SpaceSidePizzariaBLL.Models;

namespace SpaceSidePizzaria.Custom
{
    public class PizzaController : CustomController
    {
        private readonly PizzaDAO _pizzaDAO;
        private readonly PizzaBLO _pizzaBLO;
        private readonly OrderDAO _orderDAO;

        public PizzaController()
        {
            string connection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;

            _pizzaDAO = new PizzaDAO(connection);
            _pizzaBLO = new PizzaBLO();
            _orderDAO = new OrderDAO(connection);
        }

        /// <summary>
        /// Renders the pizza shop page.
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            ActionResult response = null;

            try
            {
                List<PizzaPO> prefabPizzas =
                    Mapping
                        .PizzaMapper
                        .PizzaDOListToPizzaPOList(_pizzaDAO.GetAllPrefabPizzas());

                response = View(GeneratePizzaMatrix(prefabPizzas));
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (response == null)
                {
                    response = RedirectToAction("Index", "Home");
                }
            }

            return response;
        }

        /// <summary>
        /// Renders the create form for a prefab pizza.
        /// </summary>
        [HttpGet]
        [SessionRoleFilter("Role", 1)]
        public ActionResult CreatePrefabPizza()
        {
            PizzaPO pizzaPO = PizzaWithSelectItemsFilled();

            return View(pizzaPO);
        }

        /// <summary>
        /// Posts the form to the Db.
        /// </summary>
        /// <param name="form">The user supplied info.</param>
        [HttpPost]
        [SessionRoleFilter("Role", 1)]
        public ActionResult CreatePrefabPizza(PizzaPO form)
        {
            ActionResult response = null;

            if (ModelState.IsValid)
            {
                try
                {
                    string imagesPath = "/Content/Images/"; // Path to the images folder.
                    form.Price = form.Price < 4.99M ? 4.99M : form.Price; // If the price is less than 4.99 set the price to 4.99.

                    // If the images path doesn't exist then set the form image to the NoImageAvailable picture.
                    if (!System.IO.File.Exists(Server.MapPath("~/") + form.ImagePath))
                    {
                        form.ImagePath = imagesPath + "NoImageAvailable.png";
                    }

                    // Add the new pizza to the database.
                    _pizzaDAO.AddNewPrefabPizza(Mapping.PizzaMapper.PizzaPOtoPizzaDO(form));

                    TempData["SuccessMessage"] = "The pizza prefab was succesffully added.";
                    response = RedirectToAction("PrefabPizzas", "Pizza");
                }
                catch (Exception exception)
                {
                    Logger.LogExceptionNoRepeats(exception);
                }
                finally
                {
                    if (response == null)
                    {
                        TempData["ErrorMessage"] = "An problem occured while creating the prefab. Please try again";
                        FillPizzaSelectItems(form);
                        response = View(form);
                    }
                    else { }
                }
            }
            else // The form was not valid.
            {
                FillPizzaSelectItems(form);
                response = View(form);
            }

            return response;
        }

        /// <summary>
        /// Renders the page to view the prefab pizzas.
        /// </summary>
        [HttpGet]
        [SessionRoleFilter("Role", 1)]
        public ActionResult PrefabPizzas()
        {
            ActionResult response = null;

            try
            {
                // Get all of the prefab pizzas.
                List<PizzaPO> prefabPizzas =
                    Mapping.PizzaMapper
                        .PizzaDOListToPizzaPOList(_pizzaDAO.GetAllPrefabPizzas());

                // Generate a pizza matrix for displaying materialize cards on the html page.
                // Then pass the matrix into the View.
                response = View(GeneratePizzaMatrix(prefabPizzas));
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (response == null)
                {
                    response = RedirectToAction("Index", "Home");
                }
            }

            return response;
        }

        /// <summary>
        /// Renders the update form for updating a prefab pizza.
        /// </summary>
        /// <param name="ID">The primary key for a pizza prefab.</param>
        [HttpGet]
        [SessionRoleFilter("Role", 1)]
        public ActionResult UpdatePrefabPizza(long ID)
        {
            ActionResult response = null;

            try
            {
                PizzaDO pizzaDO = _pizzaDAO.ViewPizzaByID(ID); // Get the pizza by the ID.

                if (pizzaDO != null) // If a pizza by that Id exists.
                {
                    PizzaPO pizzaPO = Mapping.PizzaMapper.PizzaDOtoPizzaPO(pizzaDO);
                    FillPizzaSelectItems(pizzaPO);

                    if (pizzaPO.OrderID == null) // If the pizza is a prefab pizza.
                    {
                        pizzaPO.Description = null;
                        response = View(pizzaPO);
                    }
                    else // It's not a prefab pizza.
                    {
                        TempData["ErrorMessage"] = "That pizza was not a prefab please choose another pizza.";
                        response = RedirectToAction("PrefabPizzas", "Pizza");
                    }
                }
                else // That pizza doesn't exist.
                {
                    RedirectingPage("The product with ID " + ID + " doesn't exist.", "");
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (response == null)
                {
                    response = RedirectToAction("Index", "Home");
                }
            }

            return response;
        }

        /// <summary>
        /// Posts the PizzaPO form the the Db.
        /// </summary>
        /// <param name="form">The user suppplied form.</param>
        [HttpPost]
        [SessionRoleFilter("Role", 1)]
        public ActionResult UpdatePrefabPizza(PizzaPO form)
        {
            ActionResult response = null;

            try
            {
                PizzaDO pizzaDO = _pizzaDAO.ViewPizzaByID(form.PizzaID);

                if (pizzaDO != null) // If that pizza exists
                {
                    PizzaPO pizzaPO = Mapping.PizzaMapper.PizzaDOtoPizzaPO(pizzaDO);

                    if (pizzaPO.OrderID == null) // If this pizza is a prefab pizza.
                    {
                        string imagesPath = "/Content/Images/"; // Path to the images folder.
                        form.Price = form.Price < 4.99M ? 4.99M : form.Price; // If the price is less than 4.99 set the price to 4.99.

                        // If the images path doesn't exist then set the form image to the NoImageAvailable picture.
                        if (!System.IO.File.Exists(Server.MapPath("~/") + form.ImagePath))
                        {
                            form.ImagePath = imagesPath + "NoImageAvailable.png";
                        }

                        _pizzaDAO.UpdatePizza(Mapping.PizzaMapper.PizzaPOtoPizzaDO(form));

                        TempData["SuccessMessage"] = "Pizza was successfully updated.";
                    }
                }
                else // The pizza doesn't exist.
                {
                    TempData["ErrorMessage"] = "That pizza doens't exist.";
                }

                response = RedirectToAction("PrefabPizzas", "Pizza");
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (response == null)
                {
                    response = RedirectToAction("Index", "Home");
                }
            }

            return response;
        }

        /// <summary>
        /// Action to delete a prefab pizza from the Db.
        /// </summary>
        /// <param name="ID">The primary key of the pizza prefab.</param>
        [HttpGet]
        [SessionRoleFilter("Role", 1)]
        public ActionResult DeletePrefabPizza(long ID)
        {
            ActionResult response = null;

            try
            {
                PizzaDO pizzaDO = _pizzaDAO.ViewPizzaByID(ID);

                if (pizzaDO != null) // If that pizza exists
                {
                    PizzaPO existingPizza = Mapping.PizzaMapper.PizzaDOtoPizzaPO(pizzaDO);

                    if (existingPizza.OrderID == null)  // If the pizza is in fact a prefab
                    {
                        _pizzaDAO.DeletePizza(ID);
                        TempData["SuccessMessage"] = "Pizza was successfully deleted";
                        response = RedirectToAction("PrefabPizzas", "Pizza");
                    }
                    else // Otherwise, the pizza the Admin is trying to delete is not a prefab pizza.
                    {
                        response = RedirectingPage("That pizza is not a prefab.", "../PrefabPizzas");
                    }
                }
                else // Otherwise, the pizza didn't exist.
                {
                    response = RedirectToAction("That pizza doesn't exist.", "../PrefabPizzas");
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (response == null)
                {
                    response = RedirectToAction("Index", "Home");
                }
            }

            return response;
        }

        /// <summary>
        /// Renders a form for creating a new pizza to add to a the cart.
        /// </summary>
        [HttpGet]
        public ActionResult CreatePizza()
        {
            return View(PizzaWithSelectItemsFilled());
        }

        /// <summary>
        /// Adds the pizza form to the cart.
        /// </summary>
        /// <param name="form">The pizza form to add to the cart.</param>
        [HttpPost]
        public ActionResult CreatePizza(PizzaPO form)
        {
            ActionResult response = null;

            try
            {
                if (ModelState.IsValid)
                {
                    SetSafePizzaValues(form);

                    // Send the pizza to the business layer for price calculation.
                    form.Price = _pizzaBLO.GetPizzaCost(Mapping.PizzaMapper.PizzaPOtoPizzaBO(form));

                    // Add the pizza to the cart.
                    (Session["Cart"] as List<PizzaPO>).Add(form);

                    response = RedirectToAction("Index", "Cart");
                }
                else // The form wasn't valid.
                {
                    TempData["ErrorMessage"] = "Please fix the problems shown below";
                    FillPizzaSelectItems(form);
                    response = View(form);
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (response == null)
                {
                    response = RedirectToAction("Index", "Home");
                }
            }

            return response;
        }

        /// <summary>
        /// Renders the form for updating a pizza in a cart.
        /// </summary>
        /// <param name="index">The index of the pizza in the cart to update.</param>
        [HttpGet]
        public ActionResult UpdatePizzaInCart(int index)
        {
            ActionResult response = null;

            try
            {
                // Get the cart from session.
                List<PizzaPO> cart = (Session["Cart"] as List<PizzaPO>);

                // If the index is NOT in bounds of the cart.
                if (index < 0 || index >= cart.Count)
                {
                    RedirectingPage("Couldn't find that Item in the Cart.", "../../Cart");
                }
                else // The index is in bounds.
                {
                    FillPizzaSelectItems(cart[index]);

                    response = View(cart[index]);

                    // Put the index is session, so we can know what index the user wanted to update.
                    Session["CartIndex"] = index;
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (response == null)
                {
                    response = RedirectToAction("Index", "Home");
                }
            }

            return response;
        }

        /// <summary>
        /// Action to update a pizza in the cart.
        /// </summary>
        /// <param name="form">The new PizzaPO.</param>
        [HttpPost]
        public ActionResult UpdatePizzaInCart(PizzaPO form)
        {
            ActionResult response = null;

            if (ModelState.IsValid)
            {
                try
                {
                    // If the CartIndex wasn't assigned in the GET UpdatePizzaInCart then
                    // the user probably tried POSTing from AJAX or something.
                    if (Session["CartIndex"] == null)
                    {
                        TempData["ErrorMessage"] = "An a problem occured will updating your pizza, try again.";
                        response = RedirectToAction("Index", "Cart");
                    }
                    else
                    {
                        int index = (int)Session["CartIndex"];  // Get the index that was stored in Session.
                        Session.Remove("CartIndex"); // Remove the "CartIndex" key from the Session.

                        SetSafePizzaValues(form);

                        // Send the pizza to the business layer for the price calculation.
                        form.Price = _pizzaBLO.GetPizzaCost(Mapping.PizzaMapper.PizzaPOtoPizzaBO(form));

                        List<PizzaPO> cart = (Session["Cart"] as List<PizzaPO>);

                        // The user could've deleted a pizza from the cart before updating, so check if the index
                        // is in the bounds of the cart.
                        if (index < 0 || index >= cart.Count)
                        {
                            TempData["ErrorMessage"] = "Something happened while updating the pizza, please try again";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Updated the pizza";
                            cart[index] = form;
                        }

                        response = RedirectToAction("Index", "Cart");
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogExceptionNoRepeats(exception);
                }
                finally
                {
                    if (response == null)
                    {
                        response = RedirectToAction("Index", "Home");
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Renders the form to update a pizza in an order.
        /// </summary>
        /// <param name="ID">The primary key of the pizza to update.</param>
        [HttpGet]
        [SessionRoleFilter("Role", 1, 2, 3)]
        public ActionResult UpdatePizzaInOrder(long ID)
        {
            ActionResult response = null;

            try
            {
                PizzaDO pizzaDOtoUpdate = _pizzaDAO.ViewPizzaByID(ID);

                if (pizzaDOtoUpdate != null)
                {
                    // This pizza exists in the database.

                    // Get the order that the pizza is associated with.
                    OrderPO pizzaOrderPO =
                        Mapping
                        .OrderMapper
                        .OrderDOtoOrderPO(
                            _orderDAO.GetOrderByID((long)pizzaDOtoUpdate.OrderID)
                        );

                    // If the current user is tied to the Pizza's order OR if the current user is an Admin.
                    if (pizzaOrderPO.UserID == GetSessionUserID() || GetSessionRole() == 1)
                    {
                        // Map the pizza the user is trying to update to a PizzaPO
                        PizzaPO pizzaPOtoUpdate = Mapping.PizzaMapper.PizzaDOtoPizzaPO(pizzaDOtoUpdate);

                        if (pizzaOrderPO.Paid) // If the order has already been paid for.
                        {
                            // Redirect the user to the order's details.

                            TempData["ErrorMessage"] = "You cannot update a pizza on an order that has already been paid for.";
                            response = RedirectToAction("OrderDetails", "Order", new { ID = pizzaOrderPO.OrderID });
                        }
                        else // Otherwise, the pizza can be updated.
                        {
                            FillPizzaSelectItems(pizzaPOtoUpdate);

                            // Pass the PizzaPO to the view.
                            response = View(pizzaPOtoUpdate);
                        }
                    }
                    else
                    {
                        // A regular user tried to update someone elses pizza.
                        Logger.Log("WARNING", "PizzaController", "UpdatePizzaInOrder",
                            "UserID: " + GetSessionUserID() + " tried to update someone else's pizza.");

                        response = RedirectToAction("MyOrders", "Order");
                    }
                }
                else // The pizza doesn't exist.
                {

                    if (GetSessionRole() == 1) // If the current user is an Admin
                    {
                        TempData["ErrorMessage"] = "That doesn't exist.";
                        RedirectToAction("ViewPendingOrders", "Order");
                    }
                    else
                    {
                        response = RedirectToAction("MyOrders", "Order");
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (response == null)
                {
                    response = RedirectToAction("Index", "Home");
                }
            }

            return response;
        }

        /// <summary>
        /// Action to update a pizza in an order.
        /// </summary>
        /// <param name="form">The new pizza.</param>
        [HttpPost]
        [SessionRoleFilter("Role", 1, 2, 3)]
        public ActionResult UpdatePizzaInOrder(PizzaPO form)
        {
            // Give response a default value.
            ActionResult response = RedirectToAction("Index", "Home");

            OrderDO pizzasOrder = _orderDAO.GetOrderByID((long)form.OrderID);

            if (pizzasOrder.Paid) // If the order has already been paid for.
            {
                TempData["ErrorMessage"] = "You cannot update a pizza on an order that has already been paid for.";
                response = RedirectToAction("OrderDetails", "Order", new { ID = pizzasOrder.OrderID });
            }
            else if (ModelState.IsValid)
            {
                if (pizzasOrder != null) // If that order exists
                {
                    // Check if the pizza form is associated with this user OR if the user is an admin
                    if (pizzasOrder.UserID == GetSessionUserID() || GetSessionRole() == 1)
                    {
                        // Get the new price for the pizza.
                        form.Price = _pizzaBLO.GetPizzaCost(Mapping.PizzaMapper.PizzaPOtoPizzaBO(form));

                        if (_pizzaDAO.UpdatePizza(Mapping.PizzaMapper.PizzaPOtoPizzaDO(form)) > 0)
                        {
                            // If the pizza was able to update then try to update the Order.

                            // First get all the pizzas associated with this order.
                            List<PizzaDO> pizzas = _pizzaDAO.GetPizzasByOrderID((long)form.OrderID);

                            // Get the total cost for the pizzas that are linked to the orderID
                            decimal newTotal = _pizzaBLO.GetCostOfPizzas(Mapping.PizzaMapper.PizzaDOListToPizzaBOList(pizzas));

                            // Update the orders total cost.
                            if (_orderDAO.UpdateOrderTotal((long)form.OrderID, newTotal)) // If updated the price
                            {
                                response = RedirectToAction("OrderDetails", "Order", new { ID = form.OrderID });
                            }
                            else // Otherwise the order is now out of sync
                            {
                                Logger.Log("WARNING", "PizzaController", "UpdatePizzaInOrder",
                                    "After trying to update a pizza in orderID: " + form.OrderID +
                                    " the total was not updated.");
                            }
                        }
                        else // Otherwise the pizza couldn't update.
                        {
                            TempData["ErrorMessage"] = "Could not update the pizza, please try again later.";
                            response = RedirectToAction("OrderDetails", "Order", new { ID = form.OrderID });
                        }
                    }
                    else // Otherwise the user shouldn't be trying to change this order.
                    {
                        Logger.Log("WARNING", "PizzaController", "UpdatePizzaInOrder",
                            "UserID: " + GetSessionUserID() + " tried to update someone elses pizza.");
                    }
                }
                else
                {
                    // That pizza doesn't exist.
                    TempData["ErrorMessage"] = "That pizza doesn't exist.";
                    response = RedirectToAction("OrderDetails", "Order", new { ID = form.OrderID });
                }
            }
            else
            {
                // The form is not valid.
                TempData["ErrorMessage"] = "Please fix the errors shown below.";
                FillPizzaSelectItems(form);

                response = View(form);
            }

            return response;

        }

        /************** HELPER METHODS START **************/

        /// <summary>
        /// Fills all of the SelectListItems in a refrence to a PizzaPO.
        /// </summary>
        /// <param name="pizzaPO">PizzaPO to fill the SelectListItems on.</param>
        private void FillPizzaSelectItems(PizzaPO pizzaPO)
        {
            pizzaPO.CrustSelectListItems = new List<SelectListItem>();
            pizzaPO.CrustSelectListItems.Add(new SelectListItem { Text = "Select a Crust Type", Value = null, Disabled = true });
            foreach (KeyValuePair<string, string> crustPairs in PizzaOptionsFactory.GetCrustDictionary())
            {
                pizzaPO.CrustSelectListItems.Add(new SelectListItem { Text = crustPairs.Key, Value = crustPairs.Value });
            }

            pizzaPO.SizeSelectListItems = new List<SelectListItem>();
            foreach (KeyValuePair<string, string> sizePairs in PizzaOptionsFactory.GetSizeDictionary())
            {
                pizzaPO.SizeSelectListItems.Add(new SelectListItem { Text = sizePairs.Key, Value = sizePairs.Value });
            }

            pizzaPO.ToppingsSelectListItems = new List<SelectListItem>();
            pizzaPO.ToppingsSelectListItems.Add(new SelectListItem { Text = "Select Toppings", Disabled = true });
            foreach (KeyValuePair<string, string> toppingPairs in PizzaOptionsFactory.GetToppingsDictionary())
            {
                pizzaPO.ToppingsSelectListItems.Add(new SelectListItem { Text = toppingPairs.Key, Value = toppingPairs.Key });
            }

            pizzaPO.SauceSelectListItems = new List<SelectListItem>();
            foreach (KeyValuePair<string, string> saucePairs in PizzaOptionsFactory.GetSauceDictionary())
            {
                pizzaPO.SauceSelectListItems.Add(new SelectListItem { Text = saucePairs.Key, Value = saucePairs.Value });
            }
        }

        /// <summary>
        /// Creates a new PizzaPO with the SelectListItems properties filled.
        /// </summary>
        /// <returns>A new PizzaPO with the SelectListItems filled.</returns>
        private PizzaPO PizzaWithSelectItemsFilled()
        {
            PizzaPO pizzaPO = new PizzaPO();

            FillPizzaSelectItems(pizzaPO);

            return pizzaPO;
        }

        /// <summary>
        /// Generates a 3xN/3 List of PizzaPOs.
        /// </summary>
        /// <param name="pizzaList">A lsit of pizzas.</param>
        /// <returns>A PizzaPO matrix. The max amount of pizzas in 1 row will be 3.</returns>
        private List<List<PizzaPO>> GeneratePizzaMatrix(List<PizzaPO> pizzaList)
        {
            List<List<PizzaPO>> pizzaMatrix = new List<List<PizzaPO>>();

            // Create a matrix of PizzaPOs for responive design.
            // Max of 3 "cards" in a row.
            for (int i = 0; i < (float)pizzaList.Count / 3; i++)
            {
                pizzaMatrix.Add(pizzaList.Skip(i * 3).Take(3).ToList());
            }

            return pizzaMatrix;
        }

        /// <summary>
        /// A convenient way to set safe values on a pizza form.
        /// </summary>
        /// <param name="pizzaPO">The refrence to the PizzaPO you wish to set the safe values on.</param>
        /// <param name="resetDescription">Determines whether or not to reset the pizza's description.</param>
        private void SetSafePizzaValues(PizzaPO pizzaPO, bool resetDescription = true)
        {
            pizzaPO.ImagePath = null;

            if (resetDescription)
            {
                pizzaPO.Description = null;
            }

            // The OrderID will be set later when the user creates an order.
            pizzaPO.OrderID = null;
        }

        /*************** HELPER METHODS END ***************/
    }
}