using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active.Models
{
    public class InteractionViewModel
    {
        public RatingModel Rating { get; set; }
        public ApplicationUser User { get; set; }
        public ActivityModel Activity { get; set; }
    }
}