using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _420_476_TP3_Gabriel_Sevigny.Models;
using System.Web.Mvc;

namespace _420_476_TP3_Gabriel_Sevigny.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // Verify the login and password entered by the user
        [HttpPost]
        public ActionResult Login(User user, string remember)
        {
            if (user.Login != null && user.Password != null)
            {
                //  Verify if the user checked the "Remember me" checkbox
                if (remember != null)
                {
                    //  Write a cookie to store the user Login.
                    Response.Cookies["RememberMe"]["Login"] = user.Login;
                    Response.Cookies["RememberMe"].Expires = DateTime.Now.AddDays(7d);    //  The cookie expires after 7 days
                }
                //  Verify if the Login and password are correct
                using (NorthwindEntities context = new NorthwindEntities())
                {
                    //  Verify if the Login exists
                    var q = from c in context.Users
                            where c.Login.Equals(user.Login)
                            select c;

                    //  Verify if the Password is correct
                    foreach (var customer in q.ToList()) {
                        byte[] data = System.Text.Encoding.ASCII.GetBytes(user.Password);
                        data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                        String hash = System.Text.Encoding.ASCII.GetString(data);
                        if (hash.Equals(customer.Password))
                        {
                            Session["loggedIn"] = true;
                            Session["Name"] = customer.FirstName + " " + customer.LastName;
                        }
                        else
                            Session["loggedIn"] = false;
                    }
                }
            }
            TempData["Message"] = "Your login or password are incorrect!";
            return RedirectToAction("Login", "Login");
        }

        //  Logout the current user
        public ActionResult Logout()
        {
            Session["loggedIn"] = false;
            Session["Name"] = null;
            return RedirectToAction("Login", "Login");
        }

        //  Verify if the user is logged in and return the result in a bool
        public static bool isLoggedIn()
        {
            if (System.Web.HttpContext.Current.Session["loggedIn"] == null)
                System.Web.HttpContext.Current.Session["loggedIn"] = false;
            return (bool)System.Web.HttpContext.Current.Session["loggedIn"];
        }

        public ActionResult Register()
        {
            return RedirectToAction("Create","User");
        }
    }
}