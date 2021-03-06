﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Active.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int Rating { get; set; }
        public string ReviewerId { get; set; }
        [ForeignKey("ReviewerId")]
        public ApplicationUser Reviewer { get; set; }
    }
}