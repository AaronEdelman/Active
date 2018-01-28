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
                if(DateTime.Now.AddHours(1) <= row.CheckinTime)
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
            newActivity.CreatorId = UserId;
            newActivity.Description = model.Description;
            newActivity.TimeStart = DateTime.Now;
            newActivity.TimeEnd = DateTime.Now.AddMinutes(model.ActivityLength);
            newActivity.Area = DistanceFinder.ConvertInviteeArea(model.Area);
            newActivity.Active = true;
            newActivity.Latitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Latitude).First();
            newActivity.Longitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Longitude).First();
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
            //create null entries UserToActivity for each invitee
            for (int i = 0; i < model.Invitees; i++)
            {
                UserToActivityModel userToActivity = new UserToActivityModel();
                userToActivity.ActivityId = model.Id;
                db.UserToActivity.Add(userToActivity);
            }
            db.Activity.Add(newActivity);
            db.SaveChanges();
            return RedirectToAction("ViewActivities");
        }

        [HttpGet]
        public ActionResult ViewActivities(UserToActivityModel model)
        {
        var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            // create available activities
            MainPageViewModel main = new MainPageViewModel();
            main.ActivityJoined = model.Id;
            main.UserId = UserId;
            main.Activities_Invitees = new List<Activity_InviteesViewModel>();
            main.UserToActivity = new List<UserToActivityModel>();
            foreach(var row in db.UserToActivity.Where(n => n.UserId == UserId))
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
                                Joiner joiner = new Joiner(invitee.User.FirstName, invitee.User.Rating, invitee.User.RatingCount);
                                string listInvitee = joiner.CreateRatingString();
                                invitees.Add(listInvitee);
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
            foreach(var rating in db.Rating)
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
                    foreach(var rating in ratings.Ratings.Where(n => n.UserId == person.UserId))
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
            ApplicationUser user = db.Users.Find(UserId);
            user.Rating += Id;
            user.RatingCount += 1;
            db.SaveChanges();
            return RedirectToAction("MyInteractions");
        }
    }
}