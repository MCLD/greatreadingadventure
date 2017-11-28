using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class ChallengesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Challenge> Challenges { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public string Categories { get; set; }
        public int? System { get; set; }
        public int? Branch { get; set; }
        public int? Program { get; set; }
        public bool? Mine { get; set; }
        public string ActiveNav { get; set; }
        public string SystemName { get; set; }
        public string BranchName { get; set; }
        public string ProgramName { get; set; }
        public bool ShowSystem { get; set; }
        public bool CanAddChallenges { get; set; }
        public bool CanDeleteChallenges { get; set; }
        public bool CanEditChallenges { get; set; }

        public IEnumerable<GRA.Domain.Model.Branch> BranchList { get; set; }
        public IEnumerable<GRA.Domain.Model.System> SystemList { get; set; }
        public IEnumerable<GRA.Domain.Model.Program> ProgramList { get; set; }

        public SelectList CategoryList { get; set; }
        public IEnumerable<int> CategoryIds { get; set; }
    }
}
