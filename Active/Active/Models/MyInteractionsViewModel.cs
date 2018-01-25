using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Active.Models
{
    public class MyInteractionsViewModel
    {
        public int Id { get; set; }
        public List<InteractionViewModel> Interactions { get; set; }
    }
}