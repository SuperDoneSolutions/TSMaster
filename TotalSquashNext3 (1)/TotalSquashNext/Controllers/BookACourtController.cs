using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

//This controller is just something a date gets passed through when a User Books a Court.

namespace TotalSquashNext.Controllers
{
    public class BookACourtController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();
        // GET: BookACourt

  
        public ActionResult BookACourt(string chosenDate)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            Session["datePicked"] = chosenDate;
            ViewBag.datePicked = chosenDate;

            return RedirectToAction("Create", "Booking");
        }
    }
}