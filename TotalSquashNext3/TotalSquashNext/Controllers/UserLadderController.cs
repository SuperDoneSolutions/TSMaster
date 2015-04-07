using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

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
            var users = db.UserLadders.Include(u => u.Ladder).Include(u => u.User).Where(x=>x.ladderId==ladderId).OrderBy(u=>u.User.wins).ThenByDescending(u=>u.User.losses);
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
            userLadder.userId = ((TotalSquashNext.Models.User)Session["currentUser"]).id;
            if (ModelState.IsValid)
            {
                db.UserLadders.Add(userLadder);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            if (ModelState.IsValid)
            {
                db.Entry(userLadder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ladderId = new SelectList(db.Ladders, "ladderId", "ladderDescription", userLadder.ladderId);
            ViewBag.userId = new SelectList(db.Users, "id", "username", userLadder.userId);
            return View(userLadder);
        }

        // GET: UserLadder/Delete/5
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
            UserLadder userLadder = db.UserLadders.Find(id);
            if (userLadder == null)
            {
                return HttpNotFound();
            }
            return View(userLadder);
        }

        // POST: UserLadder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            UserLadder userLadder = db.UserLadders.Find(id);
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
