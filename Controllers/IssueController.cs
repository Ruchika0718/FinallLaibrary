using FinallLaibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FinallLaibrary.Controllers
{
   
        public class IssueController : Controller
        {
            static int userId;
            static string userName;
            private LaibraryManagementEntities book = new LaibraryManagementEntities();
            private LaibraryManagementEntities transDb = new LaibraryManagementEntities();

            public ActionResult Index()
            {
                if (Session["userId"] == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                int userId = (int)Session["userId"];
                string userName = Session["userName"]?.ToString();

                using (var db = new LaibraryManagementEntities())
                {
                    tblUser user = db.tblUsers.Find(userId);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    IssueController.userId = userId;
                    IssueController.userName = userName;

                    return View(db.tblBooks.ToList());
                }
            }

            public ActionResult MenuBorrow()
            {
                return RedirectToAction("Index", "Issue", new { userId = userId, userName = userName });
            }

            public ActionResult MenuRequested()
            {
                return RedirectToAction("Requested", "UserTransaction", new { userId = userId });
            }
            public ActionResult MenuReceived()
            {
                Session.Remove("receivedBadge");
                return RedirectToAction("Received", "UserTransaction", new { userId = userId });
            }

            public ActionResult MenuRejected()
            {
                Session.Remove("rejectedBadge");
                return RedirectToAction("Rejected", "UserTransaction", new { userId = userId });
            }

            public ActionResult Issue(int? bookId)
            {

                if (transDb.tblTransactions.Where(t => t.UserId == userId).Count() < 6)
                {
                    if (bookId != null)
                    {
                        tblBook books = book.tblBooks.FirstOrDefault(b => b.BookId == bookId);
                        if (books == null)
                        {
                            return HttpNotFound();
                        }
                        if (books.BookCopies > 0)
                        {
                            books.BookCopies = books.BookCopies - 1;
                            tblTransaction trans = new tblTransaction()
                            {
                                BookId = books.BookId,
                                BookTitle = books.BookTitle,
                                BookISBN = books.BookISBN,
                                TranDate = DateTime.Now.ToShortDateString(),
                                TranStatus = "Requested",
                                UserId = userId,
                                UserName = userName,
                            };
                            book.SaveChanges();
                            transDb.tblTransactions.Add(trans);
                            transDb.SaveChanges();
                            Session["requestMsg"] = "Requested successfully";
                        }
                        else
                        {
                            Session["requestMsg"] = "Sorry you cant take, Book copy is zero";
                        }
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    Session["requestMsg"] = "Sorry you cant take more than six books";
                }
                return RedirectToAction("Index", "Issue", new { userId = userId, userName = userName });

            }

            public ActionResult RequestAlert()
            {
                Session.Remove("requestMsg");
                return RedirectToAction("Index", "Borrow", new { userId = userId, userName = userName });
            }
        }
    }