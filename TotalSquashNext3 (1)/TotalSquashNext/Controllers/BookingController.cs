﻿using System;
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
    public class BookingController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();


        // GET: Booking
        //Administrator - view all bookings
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
            var bookings = db.Bookings.Include(b => b.BookingType).Include(b => b.BookingRule).Include(b => b.Court);
            return View(bookings.ToList());
        }

        // GET: Booking/Details/5
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
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Booking/Create
        public ActionResult Create()
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            if (Session["datePicked"] == null)
            {
                Session["datePicked"] = "";
            }

            ViewBag.bookingCode = new SelectList(db.BookingTypes, "bookingCode", "description");
            ViewBag.buildingId = new SelectList(db.Buildings, "buildingId", "address");
            //ViewBag.bookingRulesId = new SelectList(db.BookingRules, "bookingRuleId", "bookingRuleId");
            ViewBag.courtId = new SelectList(db.Courts, "courtId", "courtDescription");
            return View();
        }

        // POST: Booking/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "courtId,bookingDate,bookingCode,userId,date,bookingRulesId,buildingId")] Booking booking)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            
            const int RULES = 1;
            

            if (ModelState.IsValid)
            {
                int userQuery = ((TotalSquashNext.Models.User)Session["currentUser"]).id;
                booking.userId = (((TotalSquashNext.Models.User)Session["currentUser"]).id);
                booking.bookingDate = DateTime.Now;
                
                booking.bookingRulesId = RULES;
                
                var dateHolder = (from x in db.Bookings
                              where x.date == booking.date
                              select x.date).Count();
                

                var dateRules = (from x in db.BookingRules
                                 where x.bookingRuleId == RULES
                                 select x.daysInAdvance).Single();

                var dayStart = (from x in db.BookingRules
                                where x.bookingRuleId == RULES
                                select x.dayStart).Single();

                var numBookings = (from x in db.Bookings
                                   where x.userId == userQuery
                                   select x).Count();

                var numBookAllowed = (from x in db.BookingRules
                                      where x.bookingRuleId == 1
                                      select x.numOfBookings).Single();

                var timeSpanRule = (from x in db.BookingRules
                                    where x.bookingRuleId == RULES
                                    select x.bookingLength).Single();

                TimeSpan bookingLength = new TimeSpan(0, timeSpanRule, 0);

                DateTime currentDate = DateTime.Now;
                DateTime datePicked = booking.date;
                DateTime checkDayRule = currentDate.AddDays((double)dateRules);
                Session["datePicked"] = datePicked;
                


                if (dateHolder == 0)
                {
                    if (datePicked <= checkDayRule && datePicked.TimeOfDay >= dayStart && numBookings < numBookAllowed)
                    {
                        
                        db.Bookings.Add(booking);
                        db.SaveChanges();
                        TempData["message"] = "Your court has been booked.";
                        return RedirectToAction("LandingPage", "Login");
                    }
                    else
                    {
                        if (datePicked > checkDayRule)
                        {
                            TempData["message"] = "Sorry friend. You cannot book more than " + dateRules.ToString() + " days in advance!";
                            return RedirectToAction("Create", "Booking");
                        }
                        else if (datePicked.TimeOfDay < dayStart)
                        {
                            TempData["message"] = "Sorry friend. You cannot book a court earlier than " + dayStart.ToString() + " am!";
                            return RedirectToAction("Create", "Booking");
                        }
                        else if (numBookings >= numBookAllowed)
                        {
                            TempData["message"] = "Sorry friend. You cannot have more than " + numBookAllowed.ToString() + " bookings!";
                            return RedirectToAction("Create", "Booking");
                        }
                    }
                }
                else
                {
                    var availCourts = (from x in db.Bookings
                                       where x.courtId != booking.courtId && x.date == null
                                       select x.courtId).ToList();

                    if (availCourts != null)
                    {
                        TempData["message"] = "Sorry that court is taken at that time. Do any of these work?";
                        ViewBag.alternateCourts = new SelectList(availCourts);
                        return RedirectToAction("AlternateCourts", "Booking");
                    }
                    else
                    {

                        List<DateTime> alternateBooking = new List<DateTime>();
                        do
                        {
                            var alternateDateHolder = (from x in db.Bookings
                                                       where x.bookingDate == ((DateTime)Session["datePicked"] + bookingLength)
                                                       select x.date).Single();

                            if (alternateDateHolder == null)
                            {
                                alternateBooking.Add(((DateTime)Session["datePicked"] + bookingLength));
                            }

                        } while (alternateBooking.Count() != 3);
                        do
                        {
                            var alternateDateHolder = (from x in db.Bookings
                                                       where x.bookingDate == ((DateTime)Session["datePicked"] - bookingLength)
                                                       select x.date).Single();
                            if (alternateDateHolder == null)
                            {
                                alternateBooking.Add(((DateTime)Session["datePicked"] - bookingLength));
                            }
                        } while (alternateBooking.Count() != 6);

                        TempData["Message"] = "Sorry nothing is available at that time. Do any of these work?";
                        ViewBag.alternateBookings = new SelectList(alternateBooking);
                        return RedirectToAction("AlternateBookings", "Booking"); // <----- Should work and should have a list of times available earlier and later than the desired time if unavailable BUT how do we call this shit??
                    }
                }

            }

            ViewBag.bookingCode = new SelectList(db.BookingTypes, "bookingCode", "description", booking.bookingCode);
            ViewBag.bookingRulesId = new SelectList(db.BookingRules, "bookingRuleId", "bookingRuleId", booking.bookingRulesId);
            ViewBag.buildingId = new SelectList(db.Buildings, "buildingId", "address");
            ViewBag.courtId = new SelectList(db.Courts, "courtId", "courtDescription", booking.courtId);
            return View(booking);
        }

        // GET: Booking/Edit/5
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
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.bookingCode = new SelectList(db.BookingTypes, "bookingCode", "description", booking.bookingCode);
            ViewBag.bookingRulesId = new SelectList(db.BookingRules, "bookingRuleId", "bookingRuleId", booking.bookingRulesId);
            ViewBag.bookingRulesId = new SelectList(db.BookingRules, "bookingRuleId", "bookingRuleId", booking.bookingRulesId);
            ViewBag.courtId = new SelectList(db.Courts, "courtId", "courtDescription", booking.courtId);
            return View(booking);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "courtId,bookingNumber,bookingDate,bookingCode,userId,date,bookingRulesId")] Booking booking)
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
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.bookingCode = new SelectList(db.BookingTypes, "bookingCode", "description", booking.bookingCode);
            ViewBag.bookingRulesId = new SelectList(db.BookingRules, "bookingRuleId", "bookingRuleId", booking.bookingRulesId);
            ViewBag.bookingRulesId = new SelectList(db.BookingRules, "bookingRuleId", "bookingRuleId", booking.bookingRulesId);
            ViewBag.courtId = new SelectList(db.Courts, "courtId", "courtDescription", booking.courtId);
            return View(booking);
        }

        // GET: Booking/Delete/5
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
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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
