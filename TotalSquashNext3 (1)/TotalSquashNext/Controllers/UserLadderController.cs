using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

//This controller is only for administrators.  Here they can create/edit/delete.  SuperDoneSolutions 2015

namespace TotalSquashNext.Controllers
{
    public class UserLadderController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        // GET: UserLadder
        // Index displays all users registered for squash ladders.
        public ActionResult Index()
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            var userLadders = db.UserLadders.Include(u => u.Ladder).Include(u => u.User).OrderBy(u=>u.User.wins).ThenByDescending(u=>u.User.losses);
            return View(userLadders.ToList());
        }

        //Displays chosen Ladder with users ordered by most wins, then least losses (eg if user1 has 10 wins and 5 losses, and user2 has 10 wins and 0 losses, user2 is higher in the ladder)
        public ActionResult DisplayByLadder(int ladderId,int userId)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }

            Session["ladderName"] = (from x in db.Ladders
                                     where x.ladderId == ladderId
                                     select x.ladderDescription).Single();

            Session["ladderId"] = ladderId;
            var users = db.UserLadders.Include(u => u.Ladder).Include(u => u.User).Where(x=>x.ladderId==ladderId).OrderByDescending(u=>u.User.wins).ThenBy(u=>u.User.losses);
            return View(users.ToList());
        }

        // GET: UserLadder/Details/5
        public ActionResult Details(int? id)
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
            UserLadder userLadder = db.UserLadders.Find(id);
            if (userLadder == null)
            {
                return HttpNotFound();
            }
            return View(userLadder);
        }

        // GET: UserLadder/Create
        public ActionResult Create()
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            ViewBag.userId = ((TotalSquashNext.Models.User)Session["currentUser"]).username;
            ViewBag.ladderId = new SelectList(db.Ladders, "ladderId", "ladderDescription");
            //ViewBag.userId = new SelectList(db.Users, "id", "username");
            return View();
        }

        // POST: UserLadder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userId,ladderId")] UserLadder userLadder)
        {

            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin","Login");
            }
            int currentUser = ((TotalSquashNext.Models.User)Session["currentUser"]).id;
            var amountOfLadders = (from x in db.LadderRules
                                  select x.numLadders).Single();
            var usersLadders = (from x in db.UserLadders
                               where x.userId == currentUser
                               select x).Count();
            if(usersLadders>=amountOfLadders)
            {
                TempData["message"] = "Current rules allow you to be registered on " + amountOfLadders + " ladders at a given time.";
                return RedirectToAction("Index", "Ladder");
            }



            userLadder.userId = ((TotalSquashNext.Models.User)Session["currentUser"]).id;

            try
            {
                if (ModelState.IsValid)
                {
                    db.UserLadders.Add(userLadder);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.ladderId = new SelectList(db.Ladders, "ladderId", "ladderDescription", userLadder.ladderId);
            //ViewBag.userId = new SelectList(db.Users, "id", "username", userLadder.userId);
            return View(userLadder);
        }

        // GET: UserLadder/Edit/5
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
            UserLadder userLadder = db.UserLadders.Find(id);
            if (userLadder == null)
            {
                return HttpNotFound();
            }
            ViewBag.ladderId = new SelectList(db.Ladders, "ladderId", "ladderDescription", userLadder.ladderId);
            ViewBag.userId = new SelectList(db.Users, "id", "username", userLadder.userId);
            return View(userLadder);
        }

        // POST: UserLadder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "position,userId,ladderId")] UserLadder userLadder)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(userLadder).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.ladderId = new SelectList(db.Ladders, "ladderId", "ladderDescription", userLadder.ladderId);
            ViewBag.userId = new SelectList(db.Users, "id", "username", userLadder.userId);
            return View(userLadder);
        }

        // GET: UserLadder/Delete/5
        public ActionResult Delete(int? id, int? ladderId)
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
            var userLadder = db.UserLadders.Where(x => x.userId == id).Where(x=>x.ladderId==ladderId).Single();
            if (userLadder == null)
            {
                return HttpNotFound();
            }
            return View(userLadder);
        }

        // POST: UserLadder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int ladderId)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }

            var userLadder = db.UserLadders.Where(x => x.userId == id).Where(x => x.ladderId == ladderId).Single();

                db.UserLadders.Remove(userLadder);

            
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
