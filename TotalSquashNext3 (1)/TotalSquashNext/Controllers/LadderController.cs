using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

//This controller is only for administrators.  Here they create/edit/delete ladders.  SuperDoneSolutions 2015

namespace TotalSquashNext.Controllers
{
    public class LadderController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        
        public ActionResult Index()
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin", "Login");
            }
            
            var ladders = db.Ladders;
            return View(ladders);
        }


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
            Ladder ladder = db.Ladders.Find(id);
            if (ladder == null)
            {
                return HttpNotFound();
            }
            return View(ladder);
        }

        // GET: Ladder/Create
        public ActionResult Create()
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
            ViewBag.ladderRuleId = new SelectList(db.LadderRules, "LadderRuleId", "LadderRuleId");
            return View();
        }

        // POST: Ladder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ladderId,ladderDescription,ladderRuleId")] Ladder ladder)
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

            try
            {
                if (ModelState.IsValid)
                {
                    ladder.ladderRuleId = 1;
                    db.Ladders.Add(ladder);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.ladderRuleId = new SelectList(db.LadderRules, "LadderRuleId", "LadderRuleId", ladder.ladderRuleId);
            return View(ladder);
        }

        // GET: Ladder/Edit/5
        public ActionResult Edit(int? id)
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
            Ladder ladder = db.Ladders.Find(id);
            if (ladder == null)
            {
                return HttpNotFound();
            }
            ViewBag.ladderRuleId = new SelectList(db.LadderRules, "LadderRuleId", "LadderRuleId", ladder.ladderRuleId);
            return View(ladder);
        }

        // POST: Ladder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ladderId,ladderDescription,ladderRuleId")] Ladder ladder)
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

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ladder).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.ladderRuleId = new SelectList(db.LadderRules, "LadderRuleId", "LadderRuleId", ladder.ladderRuleId);
            return View(ladder);
        }

        // GET: Ladder/Delete/5
        public ActionResult Delete(int? id)
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
            Ladder ladder = db.Ladders.Find(id);
            if (ladder == null)
            {
                return HttpNotFound();
            }
            return View(ladder);
        }

        // POST: Ladder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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
            Ladder ladder = db.Ladders.Find(id);
            db.Ladders.Remove(ladder);
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
