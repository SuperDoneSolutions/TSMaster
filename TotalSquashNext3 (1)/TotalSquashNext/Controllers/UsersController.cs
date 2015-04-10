using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TotalSquashNext.Models;

//This controller controls user profiles.  Users can create and edit their profiles while administrators can create/edit/delete profiles.  SuperDoneSolutions 2015

namespace TotalSquashNext.Controllers
{
    public class UsersController : Controller
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

        // GET: Users
        //Returns a list of users.
        public ActionResult Index()
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            if (((TotalSquashNext.Models.User)Session["currentUser"]).accountId != 1)
            {
                TempData["message"] = "You must be an administrator to access this page.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            var users = db.Users.Include(u => u.AccountType).Include(u => u.Country).Include(u => u.Organization).Include(u => u.Province).Include(u => u.Skill).OrderByDescending(x=>x.locked);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        //returns details of selected user - for admin only
        public ActionResult Details(int? id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            if (((TotalSquashNext.Models.User)Session["currentUser"]).accountId != 1)
            {
                TempData["message"] = "You must be an administrator to access this page.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            Session["photoUpload"] = null;
            ViewBag.accountId = new SelectList(db.AccountTypes, "accountId", "description");
            ViewBag.countryId = new SelectList(db.Countries, "countryId", "countryName");
            ViewBag.organizationId = new SelectList(db.Organizations, "organizationId", "orgName");
            ViewBag.provinceId = new SelectList(db.Provinces, "provinceId", "provinceName");
            ViewBag.skillId = new SelectList(db.Skills, "skillId", "description");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,username,skillId,password,photo,wins,losses,ties,firstName,lastName,streetAddress,city,provinceId,postalCode,countryId,phoneNumber,emailAddress,gender,birthDate,accountId,locked,organizationId,strike")] User user)
        {
            SimplerAES ep = new SimplerAES();

            try
            {
                if (ModelState.IsValid)
                {
                    int emailCheck = (from x in db.Users
                                      where x.emailAddress == user.emailAddress
                                      select x.emailAddress).Count();

                    int usernameCheck = (from x in db.Users
                                         where x.username == user.username
                                         select x.username).Count();

                    if (emailCheck > 0)
                    {
                        TempData["Message"] = "Sorry, an account with that email already exists...";
                    }
                    if (usernameCheck > 0)
                    {
                        TempData["Message"] = "Sorry, you need to pick a different Username...";
                       
                    }
                    else
                    {
                        string tempPass = user.password;
                        string encryptedPass = ep.Encrypt(tempPass);

                        user.password = encryptedPass;
                        if (user.accountId == 1)
                        {
                            user.locked = true;
                        }
                        if (Session["photoUpload"] != null)
                        {
                            user.photo = Session["photoUpload"].ToString();
                        }
                        user.strike = 0;
                        db.Users.Add(user);
                        db.SaveChanges();
                        return RedirectToAction("VerifyLogin", "Login");
                    }
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.accountId = new SelectList(db.AccountTypes, "accountId", "description", user.accountId);
            ViewBag.countryId = new SelectList(db.Countries, "countryId", "countryName", user.countryId);
            ViewBag.organizationId = new SelectList(db.Organizations, "organizationId", "orgName", user.organizationId);
            ViewBag.provinceId = new SelectList(db.Provinces, "provinceId", "provinceName", user.provinceId);
            ViewBag.skillId = new SelectList(db.Skills, "skillId", "description", user.skillId);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            Session["photoEdit"] = null;
            ViewBag.accountId = new SelectList(db.AccountTypes, "accountId", "description", user.accountId);
            ViewBag.countryId = new SelectList(db.Countries, "countryId", "countryName", user.countryId);
            ViewBag.organizationId = new SelectList(db.Organizations, "organizationId", "orgName", user.organizationId);
            ViewBag.provinceId = new SelectList(db.Provinces, "provinceId", "provinceName", user.provinceId);
            ViewBag.skillId = new SelectList(db.Skills, "skillId", "description", user.skillId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,username,skillId,password,photo,wins,losses,ties,firstName,lastName,streetAddress,city,provinceId,postalCode,countryId,phoneNumber,emailAddress,gender,birthDate,accountId,locked,organizationId")] User user)
        {
            
            SimplerAES ep = new SimplerAES();
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }

            if (ModelState.IsValid)
            {


                try
                        {
                            WebImage image = WebImage.GetImageFromRequest();
                            if (image != null)
                            {
                                String sImagePath = Request.PhysicalApplicationPath + "App_Data/" + Path.GetFileName(image.FileName);
                                image.Resize(width: 320, height: 320, preserveAspectRatio: true, preventEnlarge: true);


                                image.Save(sImagePath);

                                // now we will add it to cloudinary
                                Cloudinary cloudinary = new Cloudinary("cloudinary://347569431798466:H0Y5lsH8s9kgsklpVyOQtdA-0MY@dmxlkkyrk");
                                CloudinaryDotNet.Actions.ImageUploadParams uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
                                {
                                    File = new CloudinaryDotNet.Actions.FileDescription(sImagePath)
                                };

                                CloudinaryDotNet.Actions.ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);
                                string url = cloudinary.Api.UrlImgUp.BuildUrl(String.Format("{0}.{1}", uploadResult.PublicId, uploadResult.Format));
                                Session["photoEdit"] = url;
                                System.IO.File.Delete(sImagePath);
                                user.photo = url;


                            }
                        }
                        catch (Exception e)
                        {
                            TempData["message"] = "TRY AGAIN!!!"; 
                        }

                user.wins = (((TotalSquashNext.Models.User)Session["currentUser"]).wins);
                user.losses = (((TotalSquashNext.Models.User)Session["currentUser"]).losses);
                user.ties = (((TotalSquashNext.Models.User)Session["currentUser"]).ties);
                user.emailAddress = (((TotalSquashNext.Models.User)Session["currentUser"]).emailAddress);
                user.password = (((TotalSquashNext.Models.User)Session["currentUser"]).password);
                db.Entry(user).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("LandingPage", "Login");
            }
            ViewBag.accountId = new SelectList(db.AccountTypes, "accountId", "description", user.accountId);
            ViewBag.countryId = new SelectList(db.Countries, "countryId", "countryName", user.countryId);
            ViewBag.organizationId = new SelectList(db.Organizations, "organizationId", "orgName", user.organizationId);
            ViewBag.provinceId = new SelectList(db.Provinces, "provinceId", "provinceName", user.provinceId);
            ViewBag.skillId = new SelectList(db.Skills, "skillId", "description", user.skillId);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        //Deletes account, redirects to Index of users. only admin can see the accounts so this is okay
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", "Users");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
