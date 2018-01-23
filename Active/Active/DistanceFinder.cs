using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active
{
    public static class DistanceFinder
    {
        

        
        //converts feet/miles an activity creator inputs into maximum lat+long area
        public static float ConvertInviteeArea(float area)
        {
            float latLong = area/2.75518F*.00001F;
            return latLong;
        }
    }
}