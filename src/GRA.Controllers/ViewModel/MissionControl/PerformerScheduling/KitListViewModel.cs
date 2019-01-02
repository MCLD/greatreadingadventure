using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerScheduling
{
    public class KitListViewModel
    {
        public ICollection<PsKit> Kits { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
