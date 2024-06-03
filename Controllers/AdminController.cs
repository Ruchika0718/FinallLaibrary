using FinallLaibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinallLaibrary.Controllers
{

    // GET: Admin
        public class AdminController : Controller
        {
            LaibraryManagementEntities user = new LaibraryManagementEntities();
            public ActionResult Login()
            {
                return View();
            }
            [HttpPost]
            public ActionResult Login(tblAdmin tbl)
            {
                var admin = user.tblAdmins.SingleOrDefault(a => a.AdminEmail == tbl.AdminEmail && a.AdminPass == tbl.AdminPass);
                if (admin != null)
                {
                    var email = admin.AdminEmail;
                    var name = admin.AdminName;
                    Session["adminEmail"] = admin.AdminEmail;
                    Session["adminName"] = admin.AdminName;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Message = "User name and password do not match.";
                    return View();
                }
            }

            public ActionResult Index()
            {
                return View();
            }
            public ActionResult Logout()
            {
                Session.Remove("adminEmail");
                return RedirectToAction("Index", "Home");
            }

            public ActionResult About()
            {
                return View();
            }
            public ActionResult Contact()
            {
                return View();
            }
        public ActionResult hello()
        {
            return View();
        }
    }
    }