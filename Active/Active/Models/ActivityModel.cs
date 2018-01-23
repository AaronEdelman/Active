﻿using Active.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Active.Models
{
    public class ActivityModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Time")]
        public DateTime TimeStart { get; set; }

        [Display(Name = "End Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime TimeEnd { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Area { get; set; }
        [Display(Name = "Cost per Invitee")]
        public float CostPerUser { get; set; }
        public bool Active { get; set; }
        public int Invitees { get; set; }
        public string CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public ApplicationUser User { get; set; }
    }
}