using FinallLaibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinallLaibrary.Controllers
{
   // [Authorize]

    public class BookController : Controller
    {
        LaibraryManagementEntities book = new LaibraryManagementEntities();
        public ActionResult Index()
        {
            using (var context = new LaibraryManagementEntities())
            {
                var books = context.tblBooks.ToList();

                return View(books);
            }
        }


        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create([Bind(Include = "BookId,BookTitle,BookCategory,BookAuthor,BookCopies,BookPub,BookPubName,BookISBN,Copyright,Status")] tblBook tblBook)
        {
            if (ModelState.IsValid)
            {
                tblBook.DateAdded = DateTime.Now; // Format as a string

                Session["bookAddMsg"] = "Book Added Successfully";
                book.tblBooks.Add(tblBook);
                book.SaveChanges();
                return RedirectToAction("Index", "Book");
            }
            return View(tblBook);
        }


        public ActionResult BookAddMsg()
        {
            Session.Remove("bookAddMsg");
            return RedirectToAction("Index", "Book");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                tblBook tblBook = book.tblBooks.Find(id);
                return View(tblBook);
            }
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "BookId,BookTitle,BookCategory,BookAuthor,BookCopies,BookPub,BookPubName,BookISBN,Copyright,DateAdded,Status")] tblBook tblBook)
        {
            if (ModelState.IsValid)
            {
                Session["bookAddMsg"] = "Book updated successfully";
                book.Entry(tblBook).State = EntityState.Modified;
                book.SaveChanges();
                return RedirectToAction("Index", "Book");
            }
            return View(tblBook);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                tblBook tblBook = book.tblBooks.Find(id);
                return View(tblBook);
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
        //        tblBook tblBook = book.tblBooks.Find(id);
        //        return View(tblBook);
        //    }
        //}
        //[HttpPost]
        //public ActionResult Delete(int id)
        //{

        //    Session["bookAddMsg"] = "Book Deleted successfully";
        //    tblBook tblBook = book.tblBooks.Find(id);
        //    book.tblBooks.Remove(tblBook);
        //    book.SaveChanges();
        //    return RedirectToAction("Index", "Book");

        //}
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                // Attempt to find and delete the user
                tblBook tblBook = book.tblBooks.Find(id);
                if (tblBook != null)
                {
                    book.tblBooks.Remove(tblBook);
                    book.SaveChanges();

                    // Return success message
                    return Json(new { success = true, message = "Book deleted successfully." });
                }
                else
                {
                    // User not found
                    return Json(new { success = false, message = "Book not found." });
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