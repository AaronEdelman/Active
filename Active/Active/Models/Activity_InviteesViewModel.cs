using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Active.Models
{
    public class Activity_InviteesViewModel
    {
        public ActivityModel Activity { get; set; }
        public SelectList Invitees { get; set; }
    }
}