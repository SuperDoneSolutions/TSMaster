using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

namespace TotalSquashNext.Controllers
{
    public class BookACourtController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();
        // GET: BookACourt
        public ActionResult BookACourt([Bind(Include = "datePicked")] DateTime datePicked)
        {

            Session["datePicked"] = datePicked;

            //var dateHolder = (from x in db.Bookings
            //                  where x.bookingDate == datePicked
            //                  select x.bookingNumber).Single();

            //if(dateHolder == null)
            //{
            //    var bookingOptions
            //}

            return RedirectToAction("Create", "Booking");
        }
    }
}