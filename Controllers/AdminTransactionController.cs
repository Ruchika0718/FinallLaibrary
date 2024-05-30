using FinallLaibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FinallLaibrary.Controllers
{
    public class AdminTransactionController : Controller
    {
        private LaibraryManagementEntities transDb = new LaibraryManagementEntities();
        private LaibraryManagementEntities bookDb = new LaibraryManagementEntities();
        public ActionResult Requests()
        {
            return View(transDb.tblTransactions.ToList());
        }
        public ActionResult GetAllRequests()
        {
            var transactionList = transDb.tblTransactions.Where(r => r.TranStatus == "Requested").ToList();
            return Json(new { data = transactionList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AcceptRequest(int? tranId)
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

            transaction.TranStatus = "Accepted";
            transaction.TranDate = DateTime.Now.ToShortDateString();
            transDb.SaveChanges();

            return RedirectToAction("Requests");
        }
        public ActionResult Return()
        {
            return View(transDb.tblTransactions.ToList());
        }
        // Returns all return books in json format.
        public ActionResult GetAllReturn()
        {
            var transactionList = transDb.tblTransactions.Where(r => r.TranStatus == "Returned").ToList();
            return Json(new { data = transactionList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AcceptReturn(int? tranId)
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
            tblBook book = bookDb.tblBooks.FirstOrDefault(b => b.BookId == transaction.BookId);
            book.BookCopies = book.BookCopies + 1;
            bookDb.SaveChanges();
            transDb.tblTransactions.Remove(transaction);
            transDb.SaveChanges();
            return View("Return");

        }
    }
}