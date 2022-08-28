using AskMeDraft.Models;
using AskMeDraft.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace AskMeDraft.Controllers
{
    public class UserController : Controller
    {
        AskMeEntities db = new AskMeEntities();
        public ActionResult Login(string message)
        {
            User user = (User)Session["User"];
            if (user != null)
                return RedirectToAction("Index", "Home");
            
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.Where(x => x.Username == userLogin.Username && x.Password == userLogin.Password).FirstOrDefault();
                if (user != null)
                {
                    Session["User"] = user;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Username = userLogin.Username;
                    ViewBag.Error = "Invalid username or password";
                    return View();
                }
            }

            ViewBag.Username = userLogin.Username;

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserRegister userRegister)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.Name = userRegister.Name;
                user.Username = userRegister.Username;
                user.Email = userRegister.Email;
                user.Password = userRegister.Password;
                user.JoiningDatetime = DateTime.Now;
                user.Role = "user";
                user.Status = true;
                user.QuestionCount = 0;
                user.AnswerCount = 0;
                user.ReportCount = 0;
                db.Users.Add(user);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Login", new { message = "User registered successfully. You can now log in." });
                }
                catch (Exception e)
                {
                    if (e.ToString().Contains("Violation of UNIQUE KEY constraint 'uq_user_username'"))
                        ViewBag.Error = "Username already exists";
                    else if (e.ToString().Contains("Violation of UNIQUE KEY constraint 'uq_user_email'"))
                        ViewBag.Error = "Email already exists";
                    else
                        ViewBag.Error = "User registration failed";

                    ViewBag.Name = userRegister.Name;
                    ViewBag.Username = userRegister.Username;
                    ViewBag.Email = userRegister.Email;

                    return View();
                }
            }

            ViewBag.Name = userRegister.Name;
            ViewBag.Username = userRegister.Username;
            ViewBag.Email = userRegister.Email;
            
            return View();
        }

        public ActionResult Logout(string message)
        {
            Session.Abandon();
            return RedirectToAction("Login", new { message = message });
        }

    }
}