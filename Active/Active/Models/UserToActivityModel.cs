using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Active.Models
{
    public class UserToActivityModel
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int ActivityId { get; set; }
        [ForeignKey("ActivityId")]
        public ActivityModel Activity { get; set; }

        public string Message { get; set; }

    }
}