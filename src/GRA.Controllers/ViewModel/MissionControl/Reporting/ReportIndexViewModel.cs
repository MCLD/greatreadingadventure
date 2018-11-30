using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportIndexViewModel
    {
        public IEnumerable<Domain.Model.ReportDetails> Reports { get; set; }
        public string ReportingNote { get; set; }
    }
}
