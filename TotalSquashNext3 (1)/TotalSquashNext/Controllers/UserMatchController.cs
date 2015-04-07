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
    public class UserMatchController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        // GET: UserMatch
        public ActionResult Index(int id)
        {
            var userMatches = db.UserMatches.Include(u => u.Match).Include(u => u.User).Where(u=>u.userId==id);
            return View(userMatches.ToList());
        }

        // GET: UserMatch/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserMatch userMatch = db.UserMatches.Find(id);
            if (userMatch == null)
            {
                return HttpNotFound();
            }
            return View(userMatch);
        }

        // GET: UserMatch/Create
        public ActionResult Create()
        {
            ViewBag.gameId = new SelectList(db.Matches, "matchId", "matchId");
            ViewBag.userId = new SelectList(db.Users, "id", "username");
            return View();
        }

        // POST: UserMatch/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userId,gameId,score")] UserMatch userMatch)
        {
            if (ModelState.IsValid)
            {
                db.UserMatches.Add(userMatch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.gameId = new SelectList(db.Matches, "matchId", "matchId", userMatch.gameId);
            ViewBag.userId = new SelectList(db.Users, "id", "username", userMatch.userId);
            return View(userMatch);
        }

        // GET: UserMatch/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserMatch userMatch = db.UserMatches.Find(id);
            if (userMatch == null)
            {
                return HttpNotFound();
            }
            ViewBag.gameId = new SelectList(db.Matches, "matchId", "matchId", userMatch.gameId);
            ViewBag.userId = new SelectList(db.Users, "id", "username", userMatch.userId);
            return View(userMatch);
        }

        // POST: UserMatch/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userId,gameId,score")] UserMatch userMatch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userMatch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.gameId = new SelectList(db.Matches, "matchId", "matchId", userMatch.gameId);
            ViewBag.userId = new SelectList(db.Users, "id", "username", userMatch.userId);
            return View(userMatch);
        }

        // GET: UserMatch/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserMatch userMatch = db.UserMatches.Find(id);
            if (userMatch == null)
            {
                return HttpNotFound();
            }
            return View(userMatch);
        }

        // POST: UserMatch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserMatch userMatch = db.UserMatches.Find(id);
            db.UserMatches.Remove(userMatch);
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
