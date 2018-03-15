using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpaceSidePizzaria.Models;
using SpaceSidePizzariaDAL;
using SpaceSidePizzariaDAL.Models;
using SpaceSidePizzaria.Custom;
using System.Web.Mvc;
using System.Configuration;

namespace SpaceSidePizzaria.Controllers
{
    public class AccountController : CustomController
    {
        public AccountController()
        {
            string connection = ConfigurationManager.ConnectionStrings["dataSource"].ConnectionString;

            _userDAO = new UserDAO(connection);
        }

        private readonly UserDAO _userDAO;

        /// <summary>
        /// Renders an html page displaying every User in the DB.
        /// </summary>
        [HttpGet]
        [SessionRoleFilter("Role", 1)]
        public ActionResult Index()
        {
            ActionResult response = null;

            try
            {
                // Try getting a list of every user from the DB and pass it into the View.

                List<UserPO> userList = new List<UserPO>();

                _userDAO.GetAllUsers()
                    .ForEach(userDO => userList.Add(
                        Mapping.UserMapper.UserDOtoUserPO(userDO)));

                response = View(userList);
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
        /// Renders an html form page for registering for an account.
        /// </summary>
        [HttpGet]
        public ActionResult Register()
        {
            UserPO userPO = new UserPO();
            FillUserSelectItems(userPO);

            return View(userPO);
        }

        /// <summary>
        /// If the form passes validation, this method will create a new account for the user.
        /// </summary>
        /// <param name="form">User supplied info</param>
        [HttpPost]
        public ActionResult Register(UserPO form)
        {
            ActionResult response = null;

            if (ModelState.IsValid)
            {
                try
                {
                    UserDO existingUser = _userDAO.GetUserByUsername(form.Username);

                    if (existingUser != null) // If that user already exists in the database
                    {
                        TempData["ErrorMessage"] = "Username is taken, please choose another.";
                        FillUserSelectItems(form);
                        response = View(form);
                    }
                    else // otherwise the Username is available
                    {
                        if (GetSessionRole() != 1)
                        {
                            // If the Admin didn't fill out the form then set some of the Model
                            // properties to safe values.
                            form.RoleID = 3;
                            form.DateAdded = DateTime.Now;
                        }

                        _userDAO.AddNewUser(Mapping.UserMapper.UserPOtoUserDO(form));

                        // Try retreiving the data we just put in the database.
                        UserDO newUser = _userDAO.GetUserByUsername(form.Username);
                        if (newUser != null)  // User was added correctly
                        {
                            if (GetSessionRole() != 1 && GetSessionRole() != 2)
                            {
                                // If an employee is registering the user then don't switch accounts.
                                SetUserSession(Mapping.UserMapper.UserDOtoUserPO(newUser));
                            }
                            response = RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            Logger.Log(
                                "Warning",
                                "MVC layer", "Register Action from AccountController",
                                "A user was added to the database but this action was unable to retrieve" +
                                " any information for that user.");

                            TempData["ErrorMessage"] = "There was a problem fetching your account. Please try logging in here.";
                            response = RedirectToAction("Login", "Account");
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
                        TempData["ErrorMessage"] =
                            "There was a problem while registering your account please try again later.";
                        FillUserSelectItems(form);
                        response = View(form);
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please fix the errors shown below";
                FillUserSelectItems(form);
                response = View(form);
            }

            return response;
        }

        /// <summary>
        /// Renders a form for logging into an existing account.
        /// </summary>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// If the form passes the validation then the user's information will be stored
        /// in session.
        /// </summary>
        /// <param name="form">User supplied info</param>
        [HttpPost]
        public ActionResult Login(LoginPO form)
        {
            ActionResult response = null;
            if (ModelState.IsValid)
            {
                try
                {
                    UserDO userMatch = _userDAO.GetUserByUsername(form.Username);
                    if (userMatch != null) // We found the user in the database
                    {
                        UserPO user = Mapping.UserMapper.UserDOtoUserPO(userMatch);

                        if (form.Password == user.Password) // If the passwords match
                        {
                            SetUserSession(user);
                            response = RedirectToAction("Index", "Home");
                        }
                        else // The passwords didn't match
                        {
                            TempData["ErrorMessage"] = "The username and password don't match.";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Could not find a user by that name.";
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogExceptionNoRepeats(exception);
                    TempData["ErrorMessage"] =
                        "A problem occurred while trying to login, please try again later.";
                }
                finally
                {
                    if (response == null)
                    {
                        response = View(form);
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please fix the errors shown below.";
                response = View(form);
            }

            return response;
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Renders a page containing the user's information.
        /// </summary>
        /// <param name="ID">The primary key of the user.</param>
        [HttpGet]
        [SessionRoleFilter("Role", 1, 2, 3)]
        public ActionResult UserDetails(int ID)
        {
            ActionResult response = null;
            UserPO userPO = null;
            try
            {
                UserDO userDO = _userDAO.GetUserByID(ID);
                if (userDO != null) // If that user exists
                {
                    userPO = Mapping.UserMapper.UserDOtoUserPO(userDO);

                    // If the current User has enough privileges or the current Users
                    // ID is the same as the user that was pulled back from the database.
                    if (GetSessionRole() == 1 || GetSessionUserID() == userPO.UserID)
                    {
                        response = View(userPO);
                    }
                    else // Otherwise the User doesn't have the privileges to view this User.
                    {
                        // Show the user their own details instead.
                        userDO = _userDAO.GetUserByID(GetSessionUserID());
                        userPO = Mapping.UserMapper.UserDOtoUserPO(userDO);

                        response = View(userPO);
                    }
                }
                else // otherwise that user must not exist
                {
                    // If the User is an Admin then tell them the user doens't exist
                    // while redirecting them to the Account Index.
                    if (GetSessionRole() == 1)
                    {
                        response = RedirectingPage("Sorry, but that user doesn't exist", "../");
                    }
                    else
                    {
                        // We don't want normal users to know anything about other users.
                        // So we don't need to tell them the user doesn't exist. Instead
                        // just redirect them to their own details page.
                        userDO = _userDAO.GetUserByID(GetSessionUserID());
                        userPO = Mapping.UserMapper.UserDOtoUserPO(userDO);

                        response = View(userPO);
                    }
                }
            }
            catch (Exception exception)
            {
                TempData["ErrorMessage"] = "There was a problem viewing the users details.";
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
        /// Renders an update form for the user.
        /// </summary>
        /// <param name="ID">Primary Key of the user.</param>
        [HttpGet]
        [SessionRoleFilter("Role", 1, 3)]
        public ActionResult Update(long? ID)
        {
            ID = ID ?? GetSessionUserID();
            ActionResult response = null;

            try
            {
                UserDO userDO = _userDAO.GetUserByID((long)ID);

                if (userDO != null) // The user exists in the database
                {
                    UserPO userPO = Mapping.UserMapper.UserDOtoUserPO(userDO);

                    if (GetSessionRole() == 1 || GetSessionUserID() == userPO.UserID)
                    {
                        // The Admin should be able to update any account however,
                        // regular users should ONLY be able to update their own account.
                        FillUserSelectItems(userPO);

                        if (Request.IsAjaxRequest()) // If AJAX then send back the update partial
                        {
                            response = PartialView("~/Views/Shared/UserUpdate.cshtml", userPO);
                        }
                        else
                        {
                            response = View(userPO);
                        }
                    }
                    else // The user did not have permissions to update that account
                    {
                        response = RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    response = RedirectToAction("Index", "Home");
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
        /// Attempts to update the user's information if the validations pass.
        /// </summary>
        /// <param name="form">Primary key of the user.</param>
        [HttpPost]
        [SessionRoleFilter("Role", 1, 3)]
        public ActionResult Update(UserPO form)
        {
            ActionResult response = null;
            int rowsAffected = 0;

            try
            {
                UserDO userDOToUpdate = _userDAO.GetUserByID(form.UserID);

                if (userDOToUpdate != null) // If the user exists
                {
                    if (ModelState.IsValid)
                    {
                        UserPO userPOToUpdate = Mapping.UserMapper.UserDOtoUserPO(userDOToUpdate);

                        // Check to make sure the form "DateAdded" property is the same
                        // as the "DateAdded" column we found in the database.
                        bool isDateAddedValid = form.DateAdded.ToShortDateString() ==
                                userPOToUpdate.DateAdded.ToShortDateString();

                        if (!isDateAddedValid) // If the user tampered with the hidden "DateAdded" field
                        {
                            TempData["ErrorMessage"] = "You may not change the 'Date Added' input field.";
                            form.DateAdded = userPOToUpdate.DateAdded;
                            FillUserSelectItems(form);

                            if (Request.IsAjaxRequest())
                            {
                                response = PartialView("~/Views/Shared/UserUpdate.cshtml", form);
                            }
                            else
                            {
                                response = View(form);
                            }
                        }
                        // If the admin tried to change another admins role
                        else if (GetSessionRole() == 1 && userPOToUpdate.RoleID == 1 && form.RoleID != 1)
                        {
                            TempData["ErrorMessage"] = "You may not change another Admins Role";
                            form.RoleID = userPOToUpdate.RoleID;
                            FillUserSelectItems(form);
                            if (Request.IsAjaxRequest())
                            {
                                response = PartialView("~/Views/Shared/UserUpdate.cshtml", form);
                            }
                            else
                            {
                                response = View(form);
                            }
                        }
                        else if (GetSessionRole() == 1) // If the User is an Admin
                        {
                            rowsAffected = _userDAO.UpdateUserByID(Mapping.UserMapper.UserPOtoUserDO(form));
                            TempData["SuccessMessage"] = "Successfully updated.";
                            response = RedirectToAction("Index", "Account");
                        }
                        else // Otherwise use some validation before updating the User.
                        {
                            bool isUserIDValid = GetSessionUserID() == form.UserID,
                                 isRoleIDValid = userPOToUpdate.RoleID == form.RoleID;

                            if (isUserIDValid && isRoleIDValid)
                            {
                                rowsAffected = _userDAO.UpdateUserByID(Mapping.UserMapper.UserPOtoUserDO(form));
                            }
                            else  // The user was possibly tampering with the hidden fields
                            {
                                TempData["ErrorMessage"] = "Some fields contained invalid information.";
                                form.RoleID = userPOToUpdate.RoleID;
                                form.UserID = userPOToUpdate.UserID;

                                Logger.Log("Info", "Mvc Layer", "Update from AccountController POST Action",
                                            "A user possibly tried to change a value on a hidden input. " +
                                            "UserID: " + GetSessionUserID());
                            }
                        }
                    }
                    else // The model state is not valid.
                    {
                        FillUserSelectItems(form);
                        TempData["ErrorMessage"] = "Please fix the errors shown below";
                        if (Request.IsAjaxRequest())
                        {
                            response = PartialView("~/Views/Shared/UserUpdate.cshtml", form);
                        }
                        else
                        {
                            response = View(form);
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
                if (rowsAffected > 1) // Something terrible happened.
                {
                    Logger.Log("Warning", "Mvc Layer", "Update from AccountController POST Action",
                        "After updating UserID: " + form.UserID + " the number of rows affected was " +
                        rowsAffected);
                }
                else if (rowsAffected == 1) // Inform the User of the success.
                {
                    if (GetSessionRole() == 1) // if current user is the admin.
                    {
                        if (Request.IsAjaxRequest()) // send back JSON
                        {
                            response = Json(new { updated = true });
                        }
                        else
                        {
                            ViewBag.Message = "Successfully updated the account.";
                            Response.AppendHeader("Refresh", "3; url=../");
                        }
                    }
                    else
                    {
                        if (Request.IsAjaxRequest()) // send back JSON
                        {
                            response = Json(new { updated = true });
                        }
                        else // otherwise use the redirecting partial
                        {
                            response = RedirectingPage("Successfully updated.", "../../");
                        }
                    }
                }

                if (response == null)
                {
                    if (Request.IsAjaxRequest())
                    {
                        response = PartialView("~/Views/Shared/UserUpdate.cshtml", form);
                    }
                    else
                    {
                        response = View(form);
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Attempts to delete a user if the validation pass.
        /// </summary>
        /// <param name="ID">The primary key of the user.</param>
        /// <returns></returns>
        [HttpGet]
        [SessionRoleFilter("Role", 1, 3)]
        public ActionResult Delete(int ID)
        {
            ActionResult response = null;
            int rowsAffected = 0;

            try
            {
                UserDO userDOtoDelete = _userDAO.GetUserByID(ID);

                if (userDOtoDelete != null) // if the user exists in the database.
                {
                    // Map the userDO we got back from the database to a UserPO
                    UserPO userPOtoDelete = Mapping.UserMapper.UserDOtoUserPO(userDOtoDelete);

                    if (GetSessionRole() == 1 && userPOtoDelete.RoleID == 1)
                    {
                        // The admin is trying to delete another Admin.

                        response = RedirectingPage("Admins can't delete other admins.", "../../");

                        Logger.Log("WARNING", "AccountController", "Delete",
                            "An admin tried to delete another admin account.  " +
                            "Admin UserID: " + GetSessionUserID() + " was trying to delete " +
                            "Admin with UserID: " + ID);
                    }
                    else if (GetSessionRole() == 1 || GetSessionUserID() == ID)
                    {
                        if (userDOtoDelete.RoleID == 2) // If the user to delete is a Driver
                        {
                            rowsAffected = _userDAO.DeleteDriverByID(ID);
                        }
                        else
                        {
                            rowsAffected = _userDAO.DeleteUserByID(ID);
                        }

                        // If the User deleted their own account then, Abandon their Session.
                        if (GetSessionUserID() == ID)
                        {
                            Session.Abandon();
                        }
                    }
                    else // A user possibly tried to delete another user from the database.
                    {
                        Logger.Log("Info", "AccountController", "Delete",
                            "A user possibly tried to delete another user from the database. " +
                            "Culprit Session UserID: " + GetSessionUserID());
                    }
                }
                else
                {
                    response = RedirectingPage("That user doesn't exist.", "../../");
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (rowsAffected > 0)
                {
                    // Inform the user of the success.
                    response = RedirectingPage("Successfully deleted user.", "../../");
                }
                else
                {
                    response = RedirectingPage("Delete unsuccessful.", "../../");
                }
            }

            return response;
        }

        /// <summary>
        /// Checks the current user's account to see if the account
        /// is deliverable.
        /// </summary>
        /// <returns>A json response, with a message and invalid properties.</returns>
        [HttpGet]
        [AjaxOnlyFilter]
        [SessionRoleFilter("Role", 1, 2, 3)]
        public ActionResult CheckUserDeliverability()
        {
            JsonResult jsonResponse = Json(null, JsonRequestBehavior.AllowGet);

            try
            {
                UserPO currentUser = Mapping.UserMapper.UserDOtoUserPO(_userDAO.GetUserByID(GetSessionUserID()));
                List<string> invalidInfo = GetInvalidDeliveryInfo(currentUser);

                if (invalidInfo.Count > 0)
                {
                    string message = "Some information is required before a delivery order can be submitted: ";

                    message += string.Join(", ", invalidInfo) + '.';

                    if (GetSessionRole() == 2) // If the user is a driver.
                    {
                        message += " Your manager must update your account.";

                        jsonResponse.Data = new
                        {
                            message,
                            requiresUpdate = true,
                            employee = true
                        };
                    }
                    else // The user is not a driver
                    {
                        jsonResponse.Data = new
                        {
                            message,
                            requiresUpdate = true
                        };
                    }

                    TempData["ErrorMessage"] = message;
                }
                else // The user is oaky to make a delivery order.
                {
                    jsonResponse.Data = new
                    {
                        message = "Account is up to date.",
                        requiresUpdate = false
                    };
                }
            }
            catch (Exception exception)
            {
                Logger.LogExceptionNoRepeats(exception);
            }
            finally
            {
                if (jsonResponse.Data == null)
                {
                    jsonResponse.Data = new
                    {
                        errorMessage = "There was a problem getting the user's information."
                    };
                }
            }

            return jsonResponse;
        }

        /********** HELPER METHODS BEGIN **********/

        private void FillUserSelectItems(UserPO userPO)
        {
            userPO.RoleSelectListItems = new List<SelectListItem>();
            userPO.RoleSelectListItems.Add(new SelectListItem { Text = "3 - User", Value = "3" });
            userPO.RoleSelectListItems.Add(new SelectListItem { Text = "2 - Driver", Value = "2" });
            userPO.RoleSelectListItems.Add(new SelectListItem { Text = "1 - Admin", Value = "1" });
        }

        /*********** HELPER METHODS END ************/
    }
}