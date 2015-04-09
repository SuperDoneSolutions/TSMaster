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
    public class UserMatchController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        // GET: UserMatch
        public ActionResult Index(int id)
        {
            var userMatches = db.UserMatches.Include(u => u.Match).Include(u => u.User).Where(u => u.userId == id);
            //int currentUserId = ViewBag.userId = ((TotalSquashNext.Models.User)Session["currentUser"]).id;
            //var matches = db.Matches.Include(m => m.Booking).Where(m => m.Booking.userId == currentUserId);
            var matchIdsIncludingUser = (from x in userMatches
                                         select x.gameId).ToList();



            List<UserMatch> allUserMatches = new List<UserMatch>();

            for (int i = 0; i < matchIdsIncludingUser.Count(); i++)
            {
                int newId = matchIdsIncludingUser[i];
                var all = (from x in db.UserMatches
                           where x.gameId == newId
                           select x).ToList();
                foreach (var x in all)
                {
                    allUserMatches.Add(x);
                }

            }


            return View(allUserMatches.ToList());

            // return View(userMatches.ToList());
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

        public ActionResult CreateFromChallenge(int gameId)
        {
            
            int currentUser = ((TotalSquashNext.Models.User)Session["currentUser"]).id;
            int[] userIds = new int[2];
            userIds[0] = ((TotalSquashNext.Models.User)Session["currentUser"]).id;
            userIds[1] = ((TotalSquashNext.Models.User)Session["userToChallenge"]).id;
            for (int i = 0; i < userIds.Length; i++)
            {
                UserMatch user = new UserMatch();
                user.userId = userIds[i];
                user.gameId = gameId;
                user.score = 00;
                db.UserMatches.Add(user);
                db.SaveChanges();
            }


            return RedirectToAction("Index", new { id = currentUser });

        }
        public ActionResult UpdateScores(int gameId, int userId)
        {
            ViewBag.user = db.Users.Find(userId).username;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateScores([Bind(Include = "userId,gameId,score")] UserMatch userMatch)
        {
            
            if (ModelState.IsValid)
            {
                
                db.Entry(userMatch).State = EntityState.Modified;
                db.SaveChanges();
                CalculateWinners(userMatch.gameId);
                return RedirectToAction("Index",new { id =  ((TotalSquashNext.Models.User)Session["currentUser"]).id});
            }
            ViewBag.user = db.Users.Find(userMatch.userId).username;
            return View();
        }

        public ActionResult CalculateWinners(int gameId)
        {
            
            var game = from x in db.UserMatches
                       where x.gameId == gameId
                       select x;

            //for(int i=0;i<game.Count();i++)
            
                var players = (from x in game
                           orderby x.userId
                           select x.userId).ToList();
                var score = (from x in game
                            orderby x.userId
                            select x.score).ToList();
                var player1 = players[0];
                var player2 = players[1];
                var score1 = score[0];
                var score2 = score[1];
            if(score1==00||score2==00)
            {
                return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
            }
            if(score1>score2)
            {
                User user = db.Users.Find(player1);
                User user2 = db.Users.Find(player2);
                user.wins = user.wins++;
                user2.losses = user2.losses++;
                
                db.Entry(user).State = EntityState.Modified;
                db.Entry(user2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
            }

            if (score1 == score2)
            {
                User user = db.Users.Find(player1);
                User user2 = db.Users.Find(player2);
                user.wins = user.ties++;
                user2.losses = user2.ties++;

                db.Entry(user).State = EntityState.Modified;
                db.Entry(user2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
            }
            else
            {
                User user = db.Users.Find(player1);
                User user2 = db.Users.Find(player2);
                user2.wins = user.wins++;
                user.losses = user2.losses++;

                db.Entry(user).State = EntityState.Modified;
                db.Entry(user2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
            }

            
            

        }


        // GET: UserMatch/Create
        //Creates a match - Use only for administrators.
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
            var userMatch = db.UserMatches.Where(x => x.gameId == id);

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
            var userMatch = db.UserMatches.Where(x => x.gameId == id);
            foreach (var u in userMatch)
            {
                db.UserMatches.Remove(u);
            }
            var match = db.Matches.Where(x => x.matchId == id);
            foreach (var m in match)
            {
                db.Matches.Remove(m);
            }

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
