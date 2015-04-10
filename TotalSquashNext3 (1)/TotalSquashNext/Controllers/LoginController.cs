using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

//This controller is used for verifying users and user roles.  SuperDoneSolutions 2015

namespace TotalSquashNext.Controllers
{
    public class LoginController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        #region encrypt
        public class SimplerAES
        {
            private static byte[] key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
            private static byte[] vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };
            private ICryptoTransform encryptor, decryptor;
            private UTF8Encoding encoder;

            public SimplerAES()
            {
                RijndaelManaged rm = new RijndaelManaged();
                encryptor = rm.CreateEncryptor(key, vector);
                encoder = new UTF8Encoding();
            }

            public string Encrypt(string unencrypted)
            {
                return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
            }

            public byte[] Encrypt(byte[] buffer)
            {
                return Transform(buffer, encryptor);
            }

            protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
            {
                MemoryStream stream = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                {
                    cs.Write(buffer, 0, buffer.Length);
                }
                return stream.ToArray();
            }
        }
        #endregion

        // GET: Login

        public ActionResult VerifyLogin()
        {
            Session["currentUser"] = null;
            Session["currentOrg"] = null;
            Session["currentSkill"] = null;
            Session["currentAccount"] = null;
            Session["currentImage"] = null;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyLogin([Bind(Include = "emailAddress,password")] User user)
        {
            try
            {


                SimplerAES ep = new SimplerAES();

                if (ModelState.IsValid)
                {

                    string tempEmailVerify = user.emailAddress;
                    string tempPassVerify = ep.Encrypt(user.password);

                    if (db.Users.Where(x => x.emailAddress == tempEmailVerify).Count() > 0)
                    {
                        var passHolder = (from x in db.Users
                                          where x.emailAddress == tempEmailVerify
                                          select x.password).Single();
                        var isLocked = (from x in db.Users
                                        where x.emailAddress == tempEmailVerify
                                        select x.locked).Single();


                        if (passHolder.ToString() == tempPassVerify)
                        {
                            if (isLocked)
                            {
                                TempData["message"] = "Your account is locked until an administrator unlocks it.";
                                return RedirectToAction("VerifyLogin", "Login");
                            }

                            var currentUser = (from x in db.Users
                                               where x.emailAddress == tempEmailVerify
                                               select x).ToList();

                            User selectedUser = currentUser[0];

                            int selectedOrg = selectedUser.organizationId;
                            int selectedSkill = selectedUser.skillId;
                            int selectedAccount = selectedUser.accountId;
                            string selectedImage = selectedUser.photo;


                            Session["currentOrg"] = (from x in db.Organizations
                                                     where x.organizationId == selectedOrg
                                                     select x.orgName).Single();
                            Session["currentSkill"] = (from x in db.Skills
                                                       where x.skillId == selectedSkill
                                                       select x.description).Single();
                            Session["currentAccount"] = (from x in db.AccountTypes
                                                         where x.accountId == selectedAccount
                                                         select x.description).Single();

                            Session["currentUser"] = selectedUser;

                            Session["currentImage"] = (from x in db.Users
                                                       where x.emailAddress == tempEmailVerify
                                                       select x.photo).Single();

                            if (Session["currentImage"] == null)
                            {
                                Session["currentImage"] = "../../Images/anon.png";
                            }


                            return RedirectToAction("LandingPage");

                        }
                        else
                        {
                            TempData["message"] = "Incorrect email or password. Please try again.";
                            return RedirectToAction("VerifyLogin", "Login");
                        }

                    }
                    else
                    {
                        TempData["message"] = "Incorrect email or password. Please try again.";
                        return RedirectToAction("VerifyLogin", "Login");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Incorrect email or password. Please try again.";
                return View();
            }

            TempData["message"] = "Incorrect email or password. Please try again.";
            return View();
        }

        public ActionResult LandingPage()
        {

            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            int user = (((TotalSquashNext.Models.User)Session["currentUser"]).id);
            Session["datePicked"] = "";

            //refresh session variables.
            var currentUser = (from x in db.Users
                               where x.id == user
                               select x).ToList();

            User selectedUser = currentUser[0];
            Session["currentUser"] = selectedUser;

            

            Booking defaultBook = new Booking();
            defaultBook.date = DateTime.Today;
            defaultBook.courtId = 0;
            defaultBook.buildingId = 0;
            defaultBook.bookingNumber = 0;
            defaultBook.bookingCode = 1;
            defaultBook.bookingDate = DateTime.Today;
            defaultBook.bookingRulesId = 0;
            defaultBook.userId = user;
            Session["userBookingsOne"] = defaultBook;
            Session["userBookingsTwo"] = defaultBook;


            var userBookingOne = (from x in db.Bookings
                                  where user == x.userId
                                  orderby x.bookingNumber descending
                                  select x).FirstOrDefault();
            var userBookingTwo = (from x in db.Bookings
                                  where user == x.userId
                                  orderby x.bookingNumber descending
                                  select x).Skip(1).FirstOrDefault();
            if (userBookingOne != null)
            {
                Session["userBookingsOne"] = userBookingOne;
                if (userBookingTwo != null)
                {
                    Session["userBookingsTwo"] = userBookingTwo;
                }
                else
                {
                    Session["userBookingsTwo"] = defaultBook;
                }
            }
            else
            {
                Session["userBookingsOne"] = defaultBook;
            }
            return View();
        }
        public ActionResult AdministrativeTools()
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            if (((TotalSquashNext.Models.User)Session["currentUser"]).accountId != 1)
            {
                TempData["message"] = "You must be an administrator to access this page.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            return View();
        }


        public ActionResult ForgotPasswordSecurityCheck()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordSecurityCheck([Bind(Include = "emailAddress,birthDate,postalCode")] User user)
        {
            if (ModelState.IsValid)
            {
                var userCheck = (from x in db.Users
                                 where user.emailAddress == x.emailAddress
                                 select x).ToList();
                User selectedUser = userCheck[0];

                DateTime actualBirthDate = selectedUser.birthDate;
                string actualPostalCode = selectedUser.postalCode.ToUpper();

                if (actualBirthDate == user.birthDate && actualPostalCode == user.postalCode.ToUpper())
                {
                    return RedirectToAction("ChangePassword", new { id = selectedUser.id });
                }
                else
                {
                    TempData["message"] = "Sorry, the data you entered was incorrect. Please try again.";
                    return View();
                }

            }
            return View();
        }

        public ActionResult ChangePassword(int? id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "id,password")] User user)
        {
            SimplerAES ep = new SimplerAES();
            if (ModelState.IsValid)
            {
                var currentUser = db.Users.Find(user.id);
                string tempPass = user.password;
                string encryptedPass = ep.Encrypt(tempPass);
                currentUser.password = encryptedPass;
                db.SaveChanges();
                TempData["message"] = "Password successfully changed!";
                return RedirectToAction("VerifyLogin", "Login");

            }
            return View();
        }

    }
}