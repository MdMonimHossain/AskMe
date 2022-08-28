using AskMeDraft.Models;
using AskMeDraft.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskMeDraft.Controllers
{
    public class ProfileController : Controller
    {
        AskMeEntities db = new AskMeEntities();
        
        public ActionResult EditProfile(int? ID)
        {
            User user = (User)Session["User"];
            if (user != null)
            {
                if (ID == null)
                {
                    ViewBag.User = user;
                }
                else
                {
                    User resultUser = (from u in db.Users
                                       where u.ID == ID
                                       select u).SingleOrDefault();
                    ViewBag.User = resultUser;
                }
                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }
        
        [HttpPost]
        public ActionResult EditProfile(EditProfile editProfile)
        {
            User user = (User)Session["User"];

            User resultUser = (from u in db.Users
                               where u.ID == editProfile.ID
                               select u).SingleOrDefault();

            ViewBag.User = resultUser;

            if (ModelState.IsValid)
            {
                if(editProfile.Role == "admin" && editProfile.Status == false)
                {
                    ModelState.AddModelError("", "Admin can not be banned");
                    return View();
                }

                resultUser.Name = editProfile.Name;
                resultUser.Email = editProfile.Email;
                resultUser.Role = editProfile.Role;
                resultUser.Status = editProfile.Status;
                db.SaveChanges();

                if(user.ID == editProfile.ID)
                    Session["User"] = resultUser;

                return RedirectToAction("UserProfile", "Home", new { ID = editProfile.ID });        
            }

            return View();
        }

        public ActionResult ChangePassword()
        {
            User user = (User)Session["User"];
            if (user != null)
                return View();
            else
                return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword changePassword)
        {
            User user = (User)Session["User"];
            
            if (ModelState.IsValid)
            {
                if (user.Password == changePassword.OldPassword)
                {
                    User result = (from u in db.Users
                                     where u.ID == user.ID
                                     select u).SingleOrDefault();

                    result.Password = changePassword.NewPassword;
                    db.SaveChanges();
                    Session["User"] = result;

                    return RedirectToAction("UserProfile", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Old password is incorrect");
                    return View();
                }
                
            }
            
            return View();
        }

    }
}