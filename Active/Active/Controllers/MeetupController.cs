using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Active.Models;
using System.Data.Entity;

namespace Active.Controllers
{
    public class MeetupController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Meetup
        public ActionResult Index()
        {
            return View();
        }

        // GET: Meetup/Checkin
        [HttpGet]
        public ActionResult Checkin()
        {
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //inactivate checkins over an hour old
            foreach (var row in db.Checkin.Where(n => n.Active == true))
            {
                if (DateTime.Now >= row.CheckinTime.AddHours(1))
                {
                    row.Active = false;
                }
            }
            //inactivate activities past their experation times
            foreach (var row in db.Activity.Where(n => n.Active == true))
            {
                if (DateTime.Now >= row.TimeEnd)
                {
                    row.Active = false;
                }
            }
            db.SaveChanges();
            return View();
        }

        [HttpPost]
        public ActionResult Checkin(MainPageViewModel model)
        {
            CheckinModel newCheckin = new CheckinModel();
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            newCheckin.UserId = UserId;
            newCheckin.Active = true;
            newCheckin.CheckinTime = DateTime.Now;
            newCheckin.Latitude = model.Checkin.Latitude;
            newCheckin.Longitude = model.Checkin.Longitude;
            //Only one active checkin per user at a time
            try
            {
                foreach (var row in db.Checkin)
                    if (row.UserId == UserId && row.Active == true)
                    {
                        row.Active = false;
                    }
            }
            catch
            { }
            db.Checkin.Add(newCheckin);
            db.SaveChanges();
            return RedirectToAction("ViewActivities");
        }

        [HttpGet]
        public ActionResult CreateActivity()
        {
            return PartialView("_CreateActivity");
        }

        [HttpPost]
        public ActionResult CreateActivity(ActivityModel model)
        {
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ActivityModel newActivity = new ActivityModel();
            newActivity.Name = model.Name;
            newActivity.CostPerUser = model.CostPerUser;
            if (model.CostPerUser == 0)
            {
                newActivity.CostPerUser = 0;
            }
            newActivity.Invitees = model.Invitees;
            newActivity.CreatorId = UserId;
            newActivity.Description = model.Description;
            newActivity.TimeStart = DateTime.Now;
            newActivity.TimeEnd = DateTime.Now.AddMinutes(model.ActivityLength);
            newActivity.Area = DistanceFinder.ConvertInviteeArea(model.Area);
            newActivity.Active = true;
            newActivity.Latitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Latitude).First();
            newActivity.Longitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Longitude).First();
            //User can only have one created activity at a time
            try
            {
                foreach (var row in db.Activity)
                    if (row.CreatorId == UserId && row.Active == true)
                    {
                        row.Active = false;
                    }
            }
            catch
            { }
            //create null entries in db.UserToActivity for each invitee
            for (int i = 0; i < model.Invitees; i++)
            {
                UserToActivityModel userToActivity = new UserToActivityModel();
                userToActivity.ActivityId = model.Id;
                userToActivity.UserId = null;
                db.UserToActivity.Add(userToActivity);
            }
            db.Activity.Add(newActivity);
            db.SaveChanges();
            return RedirectToAction("ViewActivities");
        }

        [HttpGet]
        public ActionResult ViewActivities(UserToActivityModel model)
        {
            //inactivate checkins over an hour old
            foreach (var row in db.Checkin.Where(n => n.Active == true))
            {
                if (DateTime.Now >= row.CheckinTime.AddHours(1))
                {
                    row.Active = false;
                }
            }
            //inactivate activities past their expiration times
            foreach (var row in db.Activity.Where(n => n.Active == true))
            {
                if (DateTime.Now >= row.TimeEnd)
                {
                    row.Active = false;
                }
            }
            db.SaveChanges();
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            MainPageViewModel main = new MainPageViewModel();
            main.Messages = new List<string>();
            main.ActivityJoined = model.Id;
            main.UserId = UserId;
            // find out if user is checked in
            foreach (var checkin in db.Checkin.Where(n => n.Active))
            {
                if (checkin.UserId == UserId)
                {
                    main.CheckedIn = true;
                }
                else
                {
                    main.CheckedIn = false;
                }
            }
            // create available activities
            main.Activities_Invitees = new List<Activity_InviteesViewModel>();
            main.UserToActivity = new List<UserToActivityModel>();
            foreach (var row in db.UserToActivity.Where(n => n.UserId == UserId))
            {
                main.UserToActivity.Add(row);
            }
            ActivitiesViewModel activities = new ActivitiesViewModel();
            activities.Activities = new List<ActivityModel>();
            foreach (var activity in db.Activity.Where(n => n.Active == true))
            {
                activities.Activities.Add(activity);
            }
            try
            {
                double userLatitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Latitude).First();
                double userLongitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Longitude).First();
                foreach (var activity in activities.Activities)
                {
                    //find activites within creator's specified area
                    double activityDistance = DistanceFinder.FindActivitiesDistance(userLatitude, userLongitude, activity.Latitude, activity.Longitude);
                    if (activityDistance <= activity.Area)
                    {
                        Activity_InviteesViewModel activity_Invitees = new Activity_InviteesViewModel();
                        activity_Invitees.Activity = activity;
                        activity_Invitees.Distance = DistanceFinder.ConvertActivityDistance(activityDistance);
                        activity_Invitees.timeStart = activity_Invitees.Activity.TimeStart.ToShortTimeString();
                        activity_Invitees.timeEnd = activity_Invitees.Activity.TimeEnd.ToShortTimeString();
                        //find users that joined that same activity.
                        List<string> invitees = new List<string>();

                        foreach (var invitee in db.UserToActivity.Include(n => n.User))
                        {
                            if (invitee.ActivityId == activity.Id && invitee.UserId != null)
                            {
                                //Ratings
                                Joiner joiner = new Joiner(invitee.User.FirstName, invitee.User.Rating, invitee.User.RatingCount, invitee.Message);
                                string listInvitee = joiner.CreateRatingString();
                                invitees.Add(listInvitee);

                                //Messages
                                if (invitee.Message != null)
                                {
                                    string message = joiner.CreateMessageString();
                                    main.Messages.Add(message);
                                }
                            }
                        }

                        activity_Invitees.Invitees = new SelectList(invitees);

                        main.Activities_Invitees.Add(activity_Invitees);
                        ViewData["Joined"] = new SelectList(invitees);
                        foreach (var row in main.UserToActivity)
                        {
                            if (row.ActivityId == activity.Id)
                            {
                                main.ActivityJoined = row.ActivityId;
                            }
                        }
                    }
                }
            }
            catch { }
            return View(main);
        }

        [HttpGet]
        public ActionResult JoinActivities(int id)
        {
            UserToActivityModel joinedActivity = db.UserToActivity.Where(n => n.ActivityId == id).Where(n => n.UserId == null).First();
            joinedActivity.ActivityId = id;
            joinedActivity.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            db.SaveChanges();
            return RedirectToAction("ViewActivities", new { id });
        }

        public ActionResult LeaveActivity(int id)
        {
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            UserToActivityModel joinedActivity = db.UserToActivity.Where(n => n.ActivityId == id).Where(n => n.UserId == UserId).First();
            joinedActivity.UserId = null;
            db.SaveChanges();
            joinedActivity.ActivityId = 0;
            return RedirectToAction("ViewActivities");
        }
        [HttpGet]
        public ActionResult MyInteractions()
        {
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            MyInteractionsViewModel myInteractions = new MyInteractionsViewModel();
            myInteractions.Interactions = new List<InteractionViewModel>();
            UserToActivityViewModel userToActivity = new UserToActivityViewModel();
            userToActivity.UserToActivities = new List<UserToActivityModel>();
            foreach (var row in db.UserToActivity.Include(n => n.User))
            {
                userToActivity.UserToActivities.Add(row);
            }
            RatingsViewModel ratings = new RatingsViewModel();
            ratings.Ratings = new List<RatingModel>();
            foreach (var rating in db.Rating)
            {
                ratings.Ratings.Add(rating);
            }

            //Selects all activities the user has taken part in.
            foreach (var row in db.UserToActivity.Include(n => n.Activity).Where(n => n.UserId == UserId).Where(n => n.Activity.TimeEnd < DateTime.Now))
            {
                //selects all other participants in those same activities
                foreach (var person in userToActivity.UserToActivities.Where(n => n.ActivityId == row.ActivityId).Where(n => n.UserId != null))
                {
                    //do not include users previously reviewd by this user
                    InteractionViewModel interaction = new InteractionViewModel { Activity = row.Activity };
                    interaction.User = person.User;
                    if (interaction.User.Id != UserId)
                    {
                        myInteractions.Interactions.Add(interaction);
                    }
                    foreach (var rating in ratings.Ratings.Where(n => n.UserId == person.UserId))
                    {
                        if (rating.ReviewerId == UserId)
                        {
                            myInteractions.Interactions.Remove(interaction);
                        }
                    }
                }
            }
            return View(myInteractions);
        }
        public ActionResult Rate(int Id, string RateeId)
        {
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            RatingModel rating = new RatingModel();
            rating.Rating = Id;
            rating.UserId = RateeId;
            rating.ReviewerId = UserId;
            db.Rating.Add(rating);
            ApplicationUser user = db.Users.Find(RateeId);
            user.Rating += Id;
            user.RatingCount += 1;
            db.SaveChanges();
            return RedirectToAction("MyInteractions");
        }

        public ActionResult Rate_Email(int Id, string RateeId, string RateeEmail, string RateeName, string ActivityName, DateTime ActivityDate)
        {
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser sender = db.Users.Find(UserId);

            Mailgun.SendSimpleMessage(RateeEmail, RateeName, sender.FirstName, sender.LastName, ActivityName, ActivityDate.ToShortDateString(), sender.Email);
            return RedirectToAction("Rate", new { Id = 5, RateeId });
        }

        [HttpGet]
        public ActionResult EditActivity(int id)
        {
            ActivityModel activity = db.Activity.Find(id);
            return View(activity);
        }
        [HttpPost]
        public ActionResult EditActivity(ActivityModel model)
        {
            ActivityModel activity = db.Activity.Find(model.Id);
            activity.Name = model.Name;
            activity.Invitees = model.Invitees;
            activity.TimeStart = DateTime.Now;
            activity.TimeEnd = DateTime.Now.AddMinutes(model.ActivityLength);
            activity.CostPerUser = model.CostPerUser;
            activity.Area = DistanceFinder.ConvertInviteeArea(model.Area);
            db.SaveChanges();
            return RedirectToAction("ViewActivities");
        }
        public ActionResult DeleteActivity(int id)
        {
            //remove activity from db.Activity
            ActivityModel activity = db.Activity.Find(id);
            db.Activity.Remove(activity);
            //remove instances of activity from db.UsersToActivity
            foreach (var row in db.UserToActivity.Where(n => n.ActivityId == id))
            {
                db.UserToActivity.Remove(row);
            }
            db.SaveChanges();
            return RedirectToAction("ViewActivities");
        }

        [HttpGet]
        public ActionResult ViewDirections(int id)
        {
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            DirectionsViewModel directions = new DirectionsViewModel();
            ActivityModel activity = db.Activity.Find(id);
            directions.Checkin = new CheckinModel();
            directions.Activity = activity;
            double userLatitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Latitude).First();
            double userLongitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Longitude).First();
            directions.Checkin.Latitude = userLatitude;
            directions.Checkin.Longitude = userLongitude;
            return View(directions);
        }
        [HttpGet]
        public ActionResult Message()
        {
            return PartialView("Message");
        }
        [HttpPost]
        public ActionResult Message(MainPageViewModel model)
        {
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            UserToActivityModel userToActivity = db.UserToActivity.Include(n => n.Activity).Where(n => n.UserId == UserId).Where(n => n.Activity.Active == true).First();
            userToActivity.Message = model.UserId; //UserId is a holder for message here.
            db.SaveChanges();
            return RedirectToAction("ViewActivities");
        }
        //[HttpGet]
        //public ActionResult DisplayUsers()
        //{
        //    var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    double userLatitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Latitude).First();
        //    double userLongitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Longitude).First();
        //    int users100ft = 0;
        //    int users300ft = 0;
        //    int users1000ft = 0;
        //    int usersHalf = 0;
        //    int usersMile = 0;
        //    int users5Mile = 0;

        //    foreach (var activeUser in db.Checkin.Where(n => n.Active==true))
        //    {

        //        double userDistance = DistanceFinder.FindActivitiesDistance(userLatitude, userLongitude, activeUser.Latitude, activeUser.Longitude);

        //        if (userDistance < .00036)
        //        {
        //            users100ft += 1;
        //        }
        //        if (userDistance < .00109)
        //        {
        //            users300ft += 1;
        //        }
        //        if (userDistance <.00363)
        //        {
        //            users1000ft += 1;
        //        }
        //        if (userDistance <.00958)
        //        {
        //            usersHalf += 1;
        //        }
        //        if (userDistance < .01916)
        //        {
        //            usersMile += 1;
        //        }
        //        if (userDistance < .09581)
        //        {
        //            users5Mile += 1;
        //        }
        //    }
        //    string usersDistance = DistanceFinder.ConstructUsersDistance(users100ft, users300ft, users1000ft, usersHalf, usersMile, users5Mile);
        //}
    }
}