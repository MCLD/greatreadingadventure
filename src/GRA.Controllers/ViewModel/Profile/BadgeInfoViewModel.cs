using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Profile
{
    public class BadgeInfoViewModel
    {
        public UserLog UserLog { get; set; }
        public string HtmlMessage { get; set; }
        public string Message { get; set; }
    }
}
