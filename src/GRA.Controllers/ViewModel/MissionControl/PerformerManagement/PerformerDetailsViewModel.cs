using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class PerformerDetailsViewModel
    {
        public PsPerformer Performer { get; set; }

        public IEnumerable<Domain.Model.System> Systems { get; set; }
        public List<int> BranchAvailability { get; set; }
        public string BranchAvailabilityString { get; set; }

        public int BranchCount { get; set; }
        public string VendorIdPrompt { get; set; }
        public string VendorCodeFormat { get; set; }

        public bool EnablePerformerInsuranceQuestion { get; set; }
    }
}
