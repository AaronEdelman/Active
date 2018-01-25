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
            // pass activity joined Id to partial view ViewActivities
            MainPageViewModel main = new MainPageViewModel();
            //main.Activities = new List<ActivityModel>();
            //main.UserToActivity = new List<UserToActivityModel>();
            //double userLatitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Latitude).First();
            //double userLongitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Longitude).First();
            //foreach (var row in db.Activity.Where(n => n.Active == true))
            //{
            //    double activityDistance = DistanceFinder.FindActivitiesDistance(userLatitude, userLongitude, row.Latitude, row.Longitude);
            //    if(activityDistance <= row.Area)
            //    {
            //        main.Activities.Add(row);
            //    }
            //}

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

            // create available activities part of view
            MainPageViewModel main = new MainPageViewModel();
            main.ActivityJoined = model.Id;
            main.Activities = new List<ActivityModel>();
            main.UserToActivity = new List<UserToActivityModel>();
            foreach(var row in db.UserToActivity.Where(n => n.UserId == UserId))
            {
                main.UserToActivity.Add(row);
            }
            try
            {
                double userLatitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Latitude).First();
                double userLongitude = (from x in db.Checkin where x.UserId == UserId && x.Active == true select x.Longitude).First();
                foreach (var activity in db.Activity.Where(n => n.Active == true))
                {
                    double activityDistance = DistanceFinder.FindActivitiesDistance(userLatitude, userLongitude, activity.Latitude, activity.Longitude);
                    if (activityDistance <= activity.Area)
                    {
                        main.Activities.Add(activity);
                        
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
            //Selects all activities the user has taken part in.
            foreach (var row in db.UserToActivity.Include(n => n.Activity).Where(n => n.UserId == UserId).Where(n => n.Activity.TimeEnd < DateTime.Now))
            {
                //selects all other participants in those same activities
                foreach (var person in userToActivity.UserToActivities.Where(n => n.ActivityId == row.ActivityId).Where(n => n.UserId != null))
                {
                    //do not include users previously reviewd by this user
                    //string test = (from review in db.Rating where review.UserId == person.UserId && review.ReviewerId == UserId select review.UserId).First();
                    InteractionViewModel interaction = new InteractionViewModel { Activity = row.Activity };
                    interaction.User = person.User;
                    if (interaction.User.Id != UserId)
                    {
                        myInteractions.Interactions.Add(interaction);
                    }
                }
            }
            return View(myInteractions);
        }
        //public ActionResult Rate (MyInteractionsViewModel model)
        //{

        //}
    }
}