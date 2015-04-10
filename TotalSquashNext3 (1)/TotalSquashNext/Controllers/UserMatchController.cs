using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TotalSquashNext.Models;

//This controller controls scores and matches.  Users can create/edit/delete matches and can post create/delete scores but only administrators can edit scores. SuperDoneSolutions 2015

namespace TotalSquashNext.Controllers
{
    public class UserMatchController : Controller
    {
        private PrimarySquashDBContext db = new PrimarySquashDBContext();

        // GET: UserMatch
        public ActionResult Index(int id)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
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
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
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
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }

            int currentUser = ((TotalSquashNext.Models.User)Session["currentUser"]).id;
            int challengee=((TotalSquashNext.Models.User)Session["userToChallenge"]).id;
            int chosenLadderId = (int)Session["ladderId"];

            var userOnLadder = from x in db.UserLadders
                               where x.userId == currentUser
                               select x;
            
            var challengeLower= (from x in db.LadderRules
                              select x.challengeLower).Single();

            var challengeRange = (from x in db.LadderRules
                                 select x.challengeRange).Single();

            var usersLadder = (from x in db.UserLadders
                               where x.userId == currentUser
                               where x.ladderId == chosenLadderId
                               select x).Count();

            var challengeeLadders=(from x in db.UserLadders
                            where x.userId==challengee
                            where x.ladderId==chosenLadderId
                            select x).Count();

            var allUsersOnLadder = db.UserLadders.Include(x => x.User)
                .Where(x => x.ladderId == chosenLadderId).OrderByDescending(x => x.User.wins)
                .ThenBy(x => x.User.losses).ToList();

            int userPosition = allUsersOnLadder.FindIndex(x => x.userId == currentUser);
            int challengeePosition = allUsersOnLadder.FindIndex(x => x.userId == challengee);

           
            if(userOnLadder.Count()==0)
            {
                TempData["message"] = "You must be registered for at least one ladder to challenge another player.";
                return RedirectToAction("Index", "Ladder");
            }
            if(usersLadder==0 || challengeeLadders==0)
            {
                TempData["message"] = "You must both be registered on the same ladder to challenge this player.";
                return RedirectToAction("Index", "Ladder");
            }

            if((userPosition>challengeePosition)&&(challengeLower=true))
            {
                TempData["message"] = "Current rules do not allow you to challenge a user who is in a lower position on the ladder.";
                return RedirectToAction("Index", "Ladder");
            }
            if(challengeePosition-userPosition>challengeRange)
            {
                TempData["message"] = "Current rules do not allow you to challenge a user who is greater than" + challengeRange +" spaces above you on the ladder.";
                return RedirectToAction("Index", "Ladder");
            }
            if ((userPosition - challengeePosition > challengeRange) && (challengeLower=false))
            {
                TempData["message"] = "Current rules do not allow you to challenge a user who is greater than" + challengeRange + " spaces above you on the ladder.";
                return RedirectToAction("Index", "Ladder");
            }

            
            int[] userIds = new int[2];
            userIds[0] = ((TotalSquashNext.Models.User)Session["currentUser"]).id;
            userIds[1] = ((TotalSquashNext.Models.User)Session["userToChallenge"]).id;
            for (int i = 0; i < userIds.Length; i++)
            {
                UserMatch user = new UserMatch();
                user.userId = userIds[i];
                user.gameId = gameId;
                user.score = -1;
                db.UserMatches.Add(user);
                db.SaveChanges();
            }


            return RedirectToAction("Index", new { id = currentUser });

        }
        public ActionResult UpdateScores(int gameId, int userId)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            int userScore = (int)(db.UserMatches.Where(x=>x.gameId==gameId).Where(x=>x.userId==userId).Select(x=>x.score).Single());

            if (userScore > 0 && (((TotalSquashNext.Models.User)Session["currentUser"]).accountId)!=1)
            {
                TempData["message"] = "Scores cannot be edited. Please contact an administrator for assistance.";
                return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
            }
            
            ViewBag.user = db.Users.Find(userId).username;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateScores([Bind(Include = "userId,gameId,score")] UserMatch userMatch)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            try
            {
                if (ModelState.IsValid)
                {

                    db.Entry(userMatch).State = EntityState.Modified;
                    db.SaveChanges();
                    CalculateWinners(userMatch.gameId);
                    return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.user = db.Users.Find(userMatch.userId).username;
            return View();
        }

        public ActionResult CalculateWinners(int gameId)
        {
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }
            int cUser = ((TotalSquashNext.Models.User)Session["currentUser"]).id;

            
            var game = from x in db.UserMatches
                       where x.gameId == gameId
                       select x;
            
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
                User user = db.Users.Find(player1);
                User user2 = db.Users.Find(player2);
                int user1wins = (int)(user.wins);
                int user2wins = (int)user2.wins;
                int user1ties = (int)(user.ties);
                int user2ties = (int)user2.ties;
                int user1losses = (int)(user.losses);
                int user2losses = (int)user2.losses;


            //both scores have not yet been entered.
            if(score1==-1||score2==-1)
            {
                return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
            }

            //player1 won
            if(score1>score2)
            {

                user1wins++;
                user2losses++;

                user.wins = user1wins;
                user2.losses = user2losses;

               
                db.Entry(user).State = EntityState.Modified;
                db.Entry(user2).State = EntityState.Modified;
                db.SaveChanges();
                
                return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
            }
            //tie
            if (score1 == score2)
            {

                user1ties++;
                user2ties++;

                user.ties = user1ties;
                user2.ties = user2ties;


                db.Entry(user).State = EntityState.Modified;
                db.Entry(user2).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", new { id = ((TotalSquashNext.Models.User)Session["currentUser"]).id });
            }
                //player2 won
            else
            {

                user1losses++;
                user2wins++;

                user.losses = user1losses;
                user2.wins = user2wins;

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
                                db.UserMatches.Add(userMatch);
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }

                        catch
                        {
                            TempData["Message"] = "ERROR - Please try again";
                            return View();
                        }

            ViewBag.gameId = new SelectList(db.Matches, "matchId", "matchId", userMatch.gameId);
            ViewBag.userId = new SelectList(db.Users, "id", "username", userMatch.userId);
            return View(userMatch);
        }

        // GET: UserMatch/Edit/5
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
                    db.Entry(userMatch).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            catch
            {
                TempData["Message"] = "ERROR - Please try again";
                return View();
            }

            ViewBag.gameId = new SelectList(db.Matches, "matchId", "matchId", userMatch.gameId);
            ViewBag.userId = new SelectList(db.Users, "id", "username", userMatch.userId);
            return View(userMatch);
        }

        // GET: UserMatch/Delete/5
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
            if (Session["currentUser"] == null)
            {
                TempData["message"] = "Please login to continue.";
                return RedirectToAction("VerifyLogin");
            }

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
