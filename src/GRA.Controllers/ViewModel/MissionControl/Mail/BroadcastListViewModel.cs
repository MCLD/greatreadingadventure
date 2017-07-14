using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Mail
{
    public class BroadcastListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Broadcast> Broadcasts { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool Upcoming { get; set; }
    }
}
