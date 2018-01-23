using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Active.Models;

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
            CheckinModel checkin = new CheckinModel();
            return View(checkin);
        }

        [HttpPost]
        public ActionResult Checkin(CheckinModel model)
        {
            CheckinModel newCheckin = new CheckinModel();
            var UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            newCheckin.UserId = UserId;
            newCheckin.Active = true;
            newCheckin.CheckinTime = DateTime.Now;
            newCheckin.Latitude = model.Latitude;
            newCheckin.Longitude = model.Longitude;
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
            return RedirectToAction("Checkin");
        }

        [HttpGet]
        public ActionResult CreateActivity()
        {
            return PartialView("_CreateActivity");
        }
    }
}