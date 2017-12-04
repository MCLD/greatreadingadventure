using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Controllers.ViewModel.MissionControl.Dashboard
{
    public class DashboardDetailViewModel
    {
        public GRA.Domain.Model.DashboardContent DashboardContent { get; set; }
        public string Action { get; set; }
    }
}
