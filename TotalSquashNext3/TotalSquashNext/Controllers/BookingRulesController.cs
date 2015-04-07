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
    public class BookingRulesController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        // GET: BookingRules
        public ActionResult Index()
        {
            return View(db.BookingRules.ToList());
        }

        // GET: BookingRules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRule bookingRule = db.BookingRules.Find(id);
            if (bookingRule == null)
            {
                return HttpNotFound();
            }
            return View(bookingRule);
        }

        // GET: BookingRules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookingRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bookingRuleId,organizationID,daysInAdvance,numOfBookings,numOfStrikes,dayStart,bookingLength")] BookingRule bookingRule)
        {
            if (ModelState.IsValid)
            {
                db.BookingRules.Add(bookingRule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bookingRule);
        }

        // GET: BookingRules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRule bookingRule = db.BookingRules.Find(id);
            if (bookingRule == null)
            {
                return HttpNotFound();
            }
            return View(bookingRule);
        }

        // POST: BookingRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bookingRuleId,organizationID,daysInAdvance,numOfBookings,numOfStrikes,dayStart,bookingLength")] BookingRule bookingRule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookingRule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookingRule);
        }

        // GET: BookingRules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingRule bookingRule = db.BookingRules.Find(id);
            if (bookingRule == null)
            {
                return HttpNotFound();
            }
            return View(bookingRule);
        }

        // POST: BookingRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BookingRule bookingRule = db.BookingRules.Find(id);
            db.BookingRules.Remove(bookingRule);
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
