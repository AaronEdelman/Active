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
        //pythagreoum theroum
        public static double FindActivitiesDistance(double latitudeUser, double longitudeUser, double latitudeActivity, double longitudeActivity)
        {
            double diffLat = (latitudeUser - latitudeActivity) * (latitudeUser - latitudeActivity);
            double diffLong = (longitudeUser - longitudeActivity) * (longitudeUser - longitudeActivity);
            double diffLatLong = diffLat + diffLong;
            double c = (float)Math.Sqrt(diffLatLong);
            return c;
        }

        public static string ConvertActivityDistance(double area)
        {
            string distance = "0";

            double miles = 0;
            double feet = 0;
            while(area > .00001)
            {
                if (area>.01916)
                {
                    miles += 1;
                    area -= .01916;
                }
                else if (area > .00958)
                {
                    miles += .5;
                    area -= .00958;
                }
                else if (area > .00001)
                {
                    feet += (area / .00001) * 2.75518F;
                    area -= feet;
                }
            }
            if (miles > 0)
            {
                distance = miles.ToString() + " miles";
            }
            if (miles == 0)
            {
                int feetRound = (int)Math.Round(feet, 0);
                distance = feetRound.ToString() + " feet";
            }
            return distance;
        }
    }
}