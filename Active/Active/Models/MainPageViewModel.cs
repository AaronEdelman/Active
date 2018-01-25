using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active.Models
{
    public class MainPageViewModel
    {
        public List<ActivityModel> Activities { get; set; }
        public CheckinModel Checkin { get; set; }
        public RatingModel Rating { get; set; }
        public List<UserToActivityModel> UserToActivity { get; set; }
        public int ActivityJoined { get; set; }
    }
}