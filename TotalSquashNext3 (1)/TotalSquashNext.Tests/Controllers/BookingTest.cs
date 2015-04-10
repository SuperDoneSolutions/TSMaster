using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TotalSquashNext.Controllers;
using TotalSquashNext.Models;

namespace TotalSquashNext.Tests.Controllers
{
    [TestClass]
    public class BookingTest
    {
        PrimarySquashDBContext db = new PrimarySquashDBContext();

        private Booking GenericBooking()
        {
            Booking booking = new Booking();
            booking.bookingCode = 1;
            booking.bookingDate = DateTime.Now;
            booking.bookingNumber = 5;
            booking.bookingRulesId = 1;
            booking.courtId = 2;
            booking.date = DateTime.Now.AddDays(3);
            booking.userId = 1;

            return booking;
        }

        private void PurgeBooking(BookingController BookingController, string bookingNumber)
        {
            try
            {
                if (BookingController != null) BookingController.Dispose();
            }
            catch (Exception ex) { }

            PurgeBooking(bookingNumber);
        }
        private void PurgeBooking(string bookingNumber)
        {
            try
            {
                var bookings = db.Bookings.Where(a => a.bookingNumber == bookingNumber);
                foreach (Booking booking in bookings)
                {
                    db.Bookings.Remove(booking);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            { }     // ignore error            



        }
        [TestMethod]
        public void CleanBookingShouldCreate()
        {
            Booking booking = GenericBooking();
            BookingController bookingController = new BookingController();

            ActionResult result = bookingController.Create(booking);

            Assert.IsTrue(result.GetType().Name != "ViewResult");
            List<Booking> newBooking = db.Bookings.Where(a => a.bookingNumber == bookingNumber).ToList();
            Assert.IsTrue(newBooking.Count != 0);

        }
        [TestMethod]
        public void BookingDateNotInPast()
        {
            Booking booking = GenericBooking();
            booking.date = DateTime.Today.AddDays(-2);
            BookingController bookingController = new BookingController();

            ActionResult result = bookingController.Create(booking);

            //Assert
            Assert.IsTrue(result.GetType().Name == "ViewResult");
        }





    }
}
