using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Active
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional}
            );
            routes.MapRoute(
            "View Details",
            "Performances/Details/{Id}/{RateeId}",
            new
            {
                controller = "Meetup",
                action = "Rate",
                Id = UrlParameter.Optional,
                RateeId = UrlParameter.Optional
            });
            routes.MapRoute(
            "JoinActivity",
            "Meetup/JoinActivities/{Id}/{Message}",
            new
            {
                controller = "Meetup",
                action = "JoinActivities",
                Id = UrlParameter.Optional,
                Message = UrlParameter.Optional
            });
            routes.MapRoute(
            "Rate_Email",
            "Meetup/Rate_Email/{Id}/{RateeId}/{rateeEmail}/{rateeName}/{activityName}/{activityDate}",
            new
            {
                controller = "Meetup",
                action = "Rate_Email",
                Id = UrlParameter.Optional,
                RateeId = UrlParameter.Optional,
                RateeEmail = UrlParameter.Optional,
                RateeName = UrlParameter.Optional,
                ActivityName = UrlParameter.Optional,
                ActivityDate = UrlParameter.Optional,
            });
        }
    }
}
