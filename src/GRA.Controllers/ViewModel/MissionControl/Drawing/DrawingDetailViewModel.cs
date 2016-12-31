using GRA.Controllers.ViewModel.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class DrawingDetailViewModel
    {
        public GRA.Domain.Model.Drawing Drawing { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
