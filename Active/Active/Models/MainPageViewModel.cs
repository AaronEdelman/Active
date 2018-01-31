using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active.Models
{
    public class MainPageViewModel
    {
        public List<Activity_InviteesViewModel> Activities_Invitees { get; set; }
        public CheckinModel Checkin { get; set; }
        public RatingModel Rating { get; set; }
        public List<UserToActivityModel> UserToActivity { get; set; }
        public int ActivityJoined { get; set; }
        public string UserId { get; set; }
        public bool CheckedIn { get; set; }
        public List<string> Messages { get; set; }

    }
}