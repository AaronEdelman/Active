using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active
{
    public static class DistanceFinder
    {
        

        
        //converts feet/miles an activity creator inputs into maximum lat+long area
        public static double ConvertInviteeArea(double area)
        {
            double latLong = area/2.75518F*.00001D;
            return latLong;
        }

        public static double FindActivitiesDistance(double latitudeUser, double longitudeUser, double latitudeActivity, double longitudeActivity)
        {
            double diffLat = (latitudeUser - latitudeActivity) * (latitudeUser - latitudeActivity);
            double diffLong = (longitudeUser - longitudeActivity) * (longitudeUser - longitudeActivity);
            double diffLatLong = diffLat + diffLong;
            double c = (float)Math.Sqrt(diffLatLong);
            return c;
        }
    }
}