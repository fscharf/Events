﻿using Events.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Events.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public void Auth(User userModel)
        {
            using (LoginDbEntities db = new LoginDbEntities())
            {
                var userInfo = db.Users.Where(x => x.LoginEmail == userModel.LoginEmail && x.PasswordHash == userModel.PasswordHash).FirstOrDefault();
                if (userInfo == null)
                {
                    TempData["Error"] = "Usuário ou senha inválidos.";
                    Response.Redirect("/entrar");
                }
                else
                {
                    Session["Id"] = userModel.UserID;
                    Session["Email"] = userModel.LoginEmail;
                    Session["Name"] = userModel.LoginName;
                    Response.Redirect("/");
                }
            }
        }

        public void Logout()
        {
            int userId = (int)Session["Id"];
            Session.Abandon();
            Response.Redirect("/");
        }
    }
}