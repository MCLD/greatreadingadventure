using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Mail
{
    public class BroadcastDetailViewModel
    {
        public Broadcast Broadcast { get; set; }
        public bool SendNow { get; set; }
        public string Action { get; set; }
        public bool Sent { get; set; }
    }
}
