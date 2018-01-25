using Active.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Active.Models
{
    public class CheckinModel
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Checkin Time")]
        public DateTime CheckinTime { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Active { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }


    }
}