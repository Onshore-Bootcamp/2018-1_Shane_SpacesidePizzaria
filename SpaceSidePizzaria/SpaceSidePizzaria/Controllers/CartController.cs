using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpaceSidePizzaria.ViewModels;
using SpaceSidePizzaria.Models;
using SpaceSidePizzariaDAL.Models;
using SpaceSidePizzariaDAL;
using SpaceSidePizzariaBLL;
using System.Configuration;
using SpaceSidePizzaria.Custom;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace SpaceSidePizzaria.Controllers
{
    public class CartController : CustomController
    {
        private readonly PizzaDAO _pizzaDAO;
        private readonly OrderDAO _orderDAO;
        private readonly UserDAO _userDAO;
        private readonly PizzaBLO _pizzaBLO;

        public CartController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;

            _pizzaDAO = new PizzaDAO(connectionString);
            _orderDAO = new OrderDAO(connectionString);
            _userDAO = new UserDAO(connectionString);
            _pizzaBLO = new PizzaBLO();
        }

        // View items in the cart.
        [HttpGet]
        public ActionResult Index()
        {
            // Get the cart from the session.
            List<PizzaPO> cart = (Session["Cart"] as List<PizzaPO>);

            // Create a new paymentPO for the cartPayment View Model.
            PaymentPO paymentPO = new PaymentPO();

            // Get the total.
            paymentPO.Total = _pizzaBLO.GetCostOfPizzas(Mapping.PizzaMapper.PizzaPOListToPizzaBOList(cart));

            // Create the view model.
            CartPaymentVM cartPaymentVM = new CartPaymentVM(cart, paymentPO);

            return View(cartPaymentVM);
        }

        // Adds a pizza to the cart.
        [HttpGet]
        [Route("{long ID}")]
        public ActionResult AddPizzaToCart(long ID)
        {
            ActionResult response = null;

            try
            {
                PizzaDO pizzaDO = _pizzaDAO.ViewPizzaByID(ID);

                if (pizzaDO == null) //  If that prefab doesn't exist...
                {
                    // redirect to home.
                    response = RedirectToAction("Index", "Home");
                }
                else
                {
                    // First check if this pizza is actually a prefab pizza created
                    // by the Admin.  Prefabs won't have an OrderID.
                    if (pizzaDO.OrderID != null)
                    {
                        response = RedirectToAction("Index", "Pizza");
                    }
                    else
                    {
                        // Add the pizza to the cart.
                        List<PizzaPO> cart = (List<PizzaPO>)Session["Cart"];
                        cart.Add(Mapping.PizzaMapper.PizzaDOtoPizzaPO(pizzaDO));

                        TempData["SuccessMessage"] = "Item added to cart.";

                        response = RedirectToAction("Index", "Pizza");
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

        // Views the details of a pizza in the cart.
        [HttpGet]
        public ActionResult ViewPizzaDetails(int index)
        {
            ActionResult response = null;

            try
            {
                // Get the cart from Session.
                List<PizzaPO> cart = (Session["Cart"] as List<PizzaPO>);

                
                if (index < 0 || index >= cart.Count) // If the index the user is trying to delete is out of bounds.
                {
                    TempData["ErrorMessage"] = "That item doesn't exist.";
                    response = RedirectToAction("Index", "Cart");
                }
                else // Otherwise, pass the pizza to the View.
                {
                    response = View(cart[index]);
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

        // Removes a pizza from the cart.
        [HttpGet]
        public ActionResult RemovePizza(int index)
        {
            ActionResult response = null;

            try
            {
                // Get the cart from Session.
                List<PizzaPO> cart = (Session["Cart"] as List<PizzaPO>);

                if (index < 0 || index >= cart.Count) // If the index is out of bounds.
                {
                    TempData["ErrorMessage"] = "That item doesn't exist";
                }
                else // Otherwise, remove the pizza from the cart.
                {
                    cart.RemoveAt(index);
                }

                response = RedirectToAction("Index", "Cart");
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

        // Creates an order from the items in the cart.
        [HttpPost]
        [SessionRoleFilter("Role", 1, 2, 3)]
        public ActionResult CreateOrder(CartPaymentVM form)
        {
            ActionResult response = null;

            if (ModelState.IsValid)
            {
                // Make sure at least one payment method is correct.
                if (!form.PaymentPO.PayWithCash && !ValidCreditCard(form.PaymentPO.CreditCard))
                {
                    TempData["PaymentErrorMessage"] = "You must fill out the credit card info.";
                    response = RedirectToAction("Index", "Cart");
                }
                else // Otherwise, a payment method was supplied.
                {
                    try
                    {
                        // Get the cart from Session.
                        List<PizzaPO> cart = Session["Cart"] as List<PizzaPO>;

                        if (cart.Count == 0) // If the cart is empty..
                        {
                            // The user shouldn't be doing this.
                            response = RedirectToAction("Index", "Pizza");
                        }
                        else
                        {
                            // Sets up a variable to check if the user should update their account or not.
                            bool isUserInfoValid = true;

                            if (form.PaymentPO.ForDelivery) // If the user wants the order to be delivered to them.
                            {
                                // Get the current user based of the Session's UserID
                                UserPO currentUser = Mapping.UserMapper.UserDOtoUserPO(_userDAO.GetUserByID(GetSessionUserID()));

                                // Get any invalid info that is required for a delivery order to be placed.
                                List<string> invalidInfo = GetInvalidDeliveryInfo(currentUser);

                                // ** This is a fallback if the AJAX version doesn't work when the user is creating a delivery order. **
                                if (invalidInfo.Count > 0) // If there is any invalid info
                                {
                                    isUserInfoValid = false; // The user has not entered the correct information for a delivery order.

                                    string errorMessage = "Some information is required before a delivery order can be submitted: ";
                                    errorMessage += string.Join(", ", invalidInfo);

                                    if (GetSessionRole() == 2) // If the current user is a driver..
                                    {
                                        errorMessage += " Your manager must update your account.";
                                    }

                                    TempData["ErrorMessage"] = errorMessage;

                                    response = RedirectToAction("Update", "Account", new { ID = GetSessionUserID() });
                                }
                            }

                            if (isUserInfoValid) // If the user's information is correct
                            {
                                // Instantiate a new Order.
                                OrderDO newOrder = new OrderDO();

                                // Fill some of the order's properties.
                                newOrder.IsDelivery = form.PaymentPO.ForDelivery;
                                newOrder.UserID = GetSessionUserID();
                                newOrder.Status = "Prepping";
                                newOrder.OrderDate = DateTime.Now;

                                // Get the total for the order.
                                newOrder.Total = _pizzaBLO.GetCostOfPizzas(Mapping.PizzaMapper.PizzaPOListToPizzaBOList(cart));
                                
                                if (form.PaymentPO.PayWithCash)
                                {
                                    newOrder.Paid = false;
                                }
                                else
                                {
                                    newOrder.Paid = true;
                                }

                                // Get the newly created primary key after running the insert command.
                                long createdOrderID = _orderDAO.CreateOrder(newOrder);

                                if (createdOrderID > -1)
                                {
                                    // Add each pizza in the cart to the new order.
                                    foreach (PizzaPO pizzaPO in cart)
                                    {
                                        pizzaPO.OrderID = createdOrderID;
                                        PizzaDO pizzaDO = Mapping.PizzaMapper.PizzaPOtoPizzaDO(pizzaPO);

                                        if (!_pizzaDAO.AddNewPizza(pizzaDO))
                                        {
                                            Logger.Log("WARNING", "CartController", "CreateOrder",
                                                "Unable to add a pizza from the cart to the database.");
                                        }
                                        else { }
                                    }

                                    Session["Cart"] = new List<PizzaPO>(); // Create a new cart.
                                    TempData["SuccessMessage"] = "Successfully created the order.";
                                    response = RedirectToAction("MyOrders", "Order");
                                }
                                else // An execption didn't occur but the order wasn't created.
                                {
                                    TempData["ErrorMessage"] = "Something happened while creating your order, please try again.";
                                    response = RedirectToAction("Index", "Cart");
                                }
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
                }
            }
            else
            {
                // If the credit card field was not in a correct format
                if (!ModelState.IsValidField("PaymentPO.CreditCard"))
                {
                    TempData["CreditCardError"] = "Invalid credit card number.";

                } else { }

                TempData["PaymentErrorMessage"] = "Please fix the errors shown below";
                response = RedirectToAction("Index", "Cart");
            }

            return response;
        }

        /******* HELPER METHODS START ********/

        /// <summary>
        /// Checks if a string is in a valid credit card format.
        /// </summary>
        /// <param name="creditCardNum">String to validate.</param>
        /// <returns>Returns true if the creditCardNum is in a valid format, otherwise false.</returns>
        bool ValidCreditCard(string creditCardNum)
        {
            CreditCardAttribute cardAttribute = new CreditCardAttribute();

            return !String.IsNullOrEmpty(creditCardNum) && cardAttribute.IsValid(creditCardNum);
        }

        /******** HELPER METHODS END *********/
    }
}