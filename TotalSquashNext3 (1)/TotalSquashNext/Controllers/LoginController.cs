using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

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


                    if (passHolder.ToString() == tempPassVerify)
                    {
                        
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

                        if (Session["currentImage"] == null || Session["currentImage"] == "")
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
                    return HttpNotFound();
                }
            }


            return View();
        }

        public ActionResult LandingPage()
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
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
            return View();
        }
    }
}