﻿using FinallLaibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FinallLaibrary.Controllers
{
    public class UserTransactionController : Controller
    {
       
        static int userId;

        LaibraryManagementEntities transDb = new LaibraryManagementEntities();
        private LaibraryManagementEntities userDb = new LaibraryManagementEntities();

        public ActionResult Requested(int? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUser user = userDb.tblUsers.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            UserTransactionController.userId = (int)userId;
            var requestList = transDb.tblTransactions.Where(s => s.TranStatus == "Requested" && s.UserId == userId);
            if (requestList.Count() == 0)
            {
                Session["requestMessage"] = "Your Requested list is empty, Go to Borrow section for request a book.";
            }
            else
            {
                Session.Remove("requestMessage");
            }
            return View(requestList.ToList());
        }
        public ActionResult Received(int? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUser user = userDb.tblUsers.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            UserTransactionController.userId = (int)userId;
            var receivedList = transDb.tblTransactions.Where(s => s.TranStatus == "Accepted" && s.UserId == userId);
            if (receivedList.Count() == 0)
            {
                Session["receiveMessage"] = "Your Received list is empty, Wait for the admin to take action.";
            }
            else
            {
                Session.Remove("receiveMessage");
            }
            return View(receivedList.ToList());
        }
        public ActionResult ReturnReceived(int? tranId)
        {

            if (tranId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblTransaction transaction = transDb.tblTransactions.FirstOrDefault(t => t.TranId == tranId);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            transaction.TranDate = DateTime.Now.ToShortDateString();
            transaction.TranStatus = "Returned";
            transDb.SaveChanges();
            return RedirectToAction("Received", "UserTransaction", new { userId = userId });
        }
    }
}