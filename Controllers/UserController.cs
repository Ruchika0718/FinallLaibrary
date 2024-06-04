﻿using FinallLaibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinallLaibrary.Controllers
{
    //[Authorize]

    public class UserController : Controller
    {

        LaibraryManagementEntities user = new LaibraryManagementEntities();


        public ActionResult Index()
        {
            using (var context = new LaibraryManagementEntities())
            {
                var users = context.tblUsers.ToList();

                return View(users);
            }
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create([Bind(Include = "UserId,UserName,UserGender,UserDep,UserAdmNo,UserEmail,UserPass")] tblUser tblUser)
        {
            if (ModelState.IsValid)
            {
                Session["userAddMsg"] = "User Added Successfully";
                user.tblUsers.Add(tblUser);
                user.SaveChanges();
                return RedirectToAction("Index", "User");
            }
            return View();
        }
        public ActionResult UserAddMsg()
        {
            Session.Remove("userAddMsg");
            return RedirectToAction("Index", "User");
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                tblUser tblUser = user.tblUsers.Find(id);
                return View(tblUser);
            }
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "UserId,UserName,UserGender,UserDep,UserAdmNo,UserEmail,UserPass")] tblUser tblUser)
        {
            if (ModelState.IsValid)
            {
                Session["userAddMsg"] = "User updated successfully";
                user.Entry(tblUser).State = EntityState.Modified;
                user.SaveChanges();
                return RedirectToAction("Index", "User");
            }
            return View(tblUser);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                tblUser tblUser = user.tblUsers.Find(id);
                return View(tblUser);
            }
        }
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        //    }
        //    else
        //    {
        //        tblUser tblUser = user.tblUsers.Find(id);
        //        return View(tblUser);
        //    }
        //}
        //[HttpPost]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                // Attempt to find and delete the user
                tblUser tblUser = user.tblUsers.Find(id);
                if (tblUser != null)
                {
                    user.tblUsers.Remove(tblUser);
                    user.SaveChanges();

                    // Return success message
                    return Json(new { success = true, message = "User deleted successfully." });
                }
                else
                {
                    // User not found
                    return Json(new { success = false, message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the delete process
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

    }
}