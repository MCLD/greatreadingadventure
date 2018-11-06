using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class InformationViewModel
    {
        public PsPerformer Performer { get; set; }

        public List<Domain.Model.System> Systems { get; set; }
        public List<int> BranchAvailability { get; set; }
        public string BranchAvailabilityString { get; set; }

        public List<IFormFile> Images { get; set; }
        public IFormFile References { get; set; }
        public int MaxUploadMB { get; set; }

        public int BranchCount { get; set; }
    }
}
