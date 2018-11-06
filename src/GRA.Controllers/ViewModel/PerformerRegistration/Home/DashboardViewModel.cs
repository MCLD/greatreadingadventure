using System;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class DashboardViewModel
    {
        public PsPerformer Performer { get; set; }
        public PsSettings Settings { get; set; }
        public IEnumerable<Domain.Model.System> Systems { get; set; }
        public string ImagePath { get; set; }
        public string ReferencesPath { get; set; }
        public Uri Uri { get; set; }
        public bool IsEditable { get; set; }

        public List<int> BranchAvailability { get; set; }
        public IEnumerable<PsBlackoutDate> BlackoutDates { get; set; }
    }
}
