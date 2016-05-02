using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRA.Logic
{
    public class BadgeDisplayDetails
    {
        public int BadgeId { get; set; }
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
        public string AlternateText { get; set; }
        public string[] HowToEarn { get; set; }
        public string DateEarned { get; set; }
        public bool Hidden { get; set; }
        public bool Earned { get; set; }
    }
}
