using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Systems
{
     public class SystemListViewModel
    {
        public List<GRA.Domain.Model.System> Systems { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.System System { get; set; }
        public string Search { get; set; }
    }
}
