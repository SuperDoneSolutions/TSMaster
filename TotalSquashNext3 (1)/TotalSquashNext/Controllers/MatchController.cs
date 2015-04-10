using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

//Connects matches to specific bookings. Administrator has full create/edit/delete access.  SuperDoneSolutions 2015

namespace TotalSquashNext.Controllers
{
    public class MatchController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        // GET: Match
        //Displays ALL Matches. Probs best jsut for admin?
        public ActionResult Index()
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
           var matches = db.Matches.Include(m => m.Booking);
           return View(matches.ToList());
        }
        //Displays all matches for user - not including who they challenged. 
        public ActionResult UsersMatches(int id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            var matches = db.Matches.Include(m => m.Booking).Where(m=>m.Booking.userId==id);
            return View(matches.ToList());
        }

        // GET: Match/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = db.Matches.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        public ActionResult Challenge(int id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            //Current User
            ViewBag.userId = ((TotalSquashNext.Models.User)Session["currentUser"]).username;
            int currentUserId = ViewBag.userId = ((TotalSquashNext.Models.User)Session["currentUser"]).id;

            //User To Challenge
            TotalSquashNext.Models.User user = db.Users.Find(id);
            
            ViewBag.userToChallenge = user.username;
            Session["userToChallenge"] = user;

            //Current Users Bookings to connect to match.
            IQueryable<TotalSquashNext.Models.Booking> usersBookings = from x in db.Bookings
                                                                       where x.userId == currentUserId
                                                                       select x;
            if (usersBookings == null)
            {
                TempData["message"] = "You must have a court booking to challenge another player.";
                RedirectToAction("Create", "Booking");
            }

            ViewBag.bookingNumber = new SelectList(usersBookings, "bookingNumber", "date");



            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Challenge([Bind(Include = "matchId,bookingNumber")] Match match)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            var bookingMatchAlreadyExists = (from x in db.Matches
                                            where x.bookingNumber == match.bookingNumber
                                            select x).Count();
            if(bookingMatchAlreadyExists>0)
            {
                TempData["message"] = "You can only have one match per booking, and you already have a match set for this booking.";
                return RedirectToAction("Index", "Ladder");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    db.Matches.Add(match);
                    db.SaveChanges();
                    return RedirectToAction("CreateFromChallenge", "UserMatch", new { gameId = match.matchId });
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }
            //Current User
            ViewBag.userId = ((TotalSquashNext.Models.User)Session["currentUser"]).username;
            int currentUserId = ViewBag.userId = ((TotalSquashNext.Models.User)Session["currentUser"]).id;

            //User To Challenge

            //Current Users Bookings to connect to match.
            IQueryable<TotalSquashNext.Models.Booking> usersBookings = from x in db.Bookings
                                                                       where x.userId == currentUserId
                                                                       select x;

            ViewBag.bookingNumber = new SelectList(usersBookings, "bookingNumber", "date");
            return RedirectToAction("CreateFromChallenge", "UserMatch", new { gameId=match.matchId});
        }


        // GET: Match/Create
        public ActionResult Create()
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            ViewBag.bookingNumber = new SelectList(db.Bookings, "bookingNumber", "bookingNumber");
            return View();
        }

        // POST: Match/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "matchId,bookingNumber")] Match match)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Matches.Add(match);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.bookingNumber = new SelectList(db.Bookings, "bookingNumber", "bookingNumber", match.bookingNumber);
            return View(match);
        }

        // GET: Match/Edit/5
        public ActionResult Edit(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = db.Matches.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.bookingNumber = new SelectList(db.Bookings, "bookingNumber", "bookingNumber", match.bookingNumber);
            return View(match);
        }

        // POST: Match/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "matchId,bookingNumber")] Match match)
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
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(match).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.bookingNumber = new SelectList(db.Bookings, "bookingNumber", "bookingNumber", match.bookingNumber);
            return View(match);
        }

        // GET: Match/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = db.Matches.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // POST: Match/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }

            Match match = db.Matches.Find(id);
            var userMatches = db.UserMatches.Where(x => x.gameId == id);
            foreach(var m in userMatches)
            {
                db.UserMatches.Remove(m);
            }
            
            db.Matches.Remove(match);
            db.SaveChanges();
            return RedirectToAction("Index");
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
