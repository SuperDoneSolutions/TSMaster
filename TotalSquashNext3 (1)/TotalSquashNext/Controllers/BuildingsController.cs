using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

//This controller is only for administrators. Here they add buildings that contain squash courts (when applicable).  SuperDoneSolutions 2015

namespace TotalSquashNext.Controllers
{
    public class BuildingsController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        // GET: Buildings
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
            var buildings = db.Buildings.Include(b => b.Organization);
            return View(buildings.ToList());
        }

        // GET: Buildings/Details/5
        public ActionResult Details(int? id)
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
            Building building = db.Buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            return View(building);
        }

        // GET: Buildings/Create
        public ActionResult Create()
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
            ViewBag.organizationId = new SelectList(db.Organizations, "organizationId", "orgName");
            return View();
        }

        // POST: Buildings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "buildingId,address,organizationId")] Building building)
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
                    db.Buildings.Add(building);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }
            
            ViewBag.organizationId = new SelectList(db.Organizations, "organizationId", "orgName", building.organizationId);
            return View(building);
        }

        // GET: Buildings/Edit/5
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
            Building building = db.Buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            ViewBag.organizationId = new SelectList(db.Organizations, "organizationId", "orgName", building.organizationId);
            return View(building);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "buildingId,address,organizationId")] Building building)
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
                    db.Entry(building).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }
            
            ViewBag.organizationId = new SelectList(db.Organizations, "organizationId", "orgName", building.organizationId);
            return View(building);
        }

        // GET: Buildings/Delete/5
        public ActionResult Delete(int? id)
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
            Building building = db.Buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            return View(building);
        }

        // POST: Buildings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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
            Building building = db.Buildings.Find(id);
            db.Buildings.Remove(building);
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
