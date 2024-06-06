using FinallLaibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace FinallLaibrary.Controllers
{
    // [Authorize]
    public class StudentController : Controller
    {
        // GET: Student
        LaibraryManagementEntities user = new LaibraryManagementEntities();





        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(tblUser tbl)
        {
            if (ModelState.IsValid)
            {
                var existingUser = user.tblUsers.SingleOrDefault(u => u.UserEmail == tbl.UserEmail);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserEmail", "Please enter a different email.");
                    return View(tbl);
                }

                var newUser = new tblUser
                {
                    UserName = tbl.UserName,
                    UserGender = tbl.UserGender,
                    UserDep = tbl.UserDep,
                    UserAdmNo = tbl.UserAdmNo,
                    UserEmail = tbl.UserEmail,
                    UserPass = tbl.UserPass,
                    IsActive = true  
                };

                user.tblUsers.Add(newUser);
                user.SaveChanges();

                return RedirectToAction("Login", "Student");
            }

            return View(tbl);
        }

        //[HttpPost]
        //public ActionResult Registration(tblUser tbl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var existingUser = user.tblUsers.SingleOrDefault(u => u.UserEmail == tbl.UserEmail);
        //        if (existingUser != null)
        //        {
        //            ModelState.AddModelError("UserEmail", " Please enter a different email.");
        //            return View(tbl);
        //        }
        //        var student = new tblUser
        //        {
        //            UserName = tbl.UserName,
        //            UserGender = tbl.UserGender,
        //            UserDep = tbl.UserDep,
        //            UserAdmNo = tbl.UserAdmNo,
        //            UserEmail = tbl.UserEmail,
        //            UserPass = tbl.UserPass,
        //        };

        //        user.tblUsers.Add(tbl);
        //        user.SaveChanges();

        //        return RedirectToAction("Login", "Student");
        //    }

        //    return View(tbl);
        //}
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(tblUser user)
        {
            if (string.IsNullOrEmpty(user.UserEmail) || string.IsNullOrEmpty(user.UserPass))
            {
                ViewBag.Message = "Please enter both email and password.";
                return View();
            }

            using (var db = new LaibraryManagementEntities())
            {
                var loginUser = await db.tblUsers.SingleOrDefaultAsync(u => u.UserEmail == user.UserEmail && u.UserPass == user.UserPass);

                if (loginUser != null)
                {
                    if (loginUser.IsActive == true)
                    {
                        Session["userId"] = loginUser.UserId;
                        Session["userName"] = loginUser.UserName;
                        return RedirectToAction("Index", "Student");
                    }
                    else
                    {
                        ViewBag.Message = "You can't login because you are not active. Please contact admin.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Email and password do not match.";
                    return View();
                }
            }
        }


        public ActionResult Logout()
        {
            Session.Remove("userId");
            Session.Remove("userName");
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();

        }
        public ActionResult Contact()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string UserEmail)
        {
            string message = "";
            bool status = false;

            using (LaibraryManagementEntities dc = new LaibraryManagementEntities())
            {
                var account = dc.tblUsers.Where(a => a.UserEmail == UserEmail).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.UserEmail, resetCode, "ResetPassword");
                    account.ResetCode = resetCode;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Student/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("ruchimpanchal@gmail.com", "ldyotgflrdceusnc");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "ldyotgflrdceusnc"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);


        }
        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (LaibraryManagementEntities dc = new LaibraryManagementEntities())
            {
                var user = dc.tblUsers.Where(a => a.ResetCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPassword model = new ResetPassword();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }
        [HttpPost]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";

            if (ModelState.IsValid)
            {
                using (LaibraryManagementEntities dc = new LaibraryManagementEntities())
                {
                    try
                    {
                        var user = dc.tblUsers.FirstOrDefault(a => a.ResetCode == model.ResetCode);
                        if (user != null)
                        {
                            user.UserPass = model.NewPassword;
                            user.ResetCode = null;

                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();
                            dc.Configuration.ValidateOnSaveEnabled = true;

                            message = "New password updated successfully.";
                        }
                        else
                        {
                            message = "Invalid reset code.";
                        }
                    }
                    catch (Exception ex)
                    {
                        message = "An error occurred while updating the password. Please try again later.";
                    }
                }
            }
            else
            {
                message = "Invalid data provided.";
            }

            // Pass the message to the view
            ViewBag.Message = message;
            return RedirectToAction("Index", "Student");
        }
    }
}