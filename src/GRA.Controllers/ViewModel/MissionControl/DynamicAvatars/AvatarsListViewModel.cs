using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.DynamicAvatars
{
    public class AvatarsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.DynamicAvatar> Avatars { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public bool CanAddAvatars { get; set; }
        public bool CanDeleteAvatars { get; set; }
        public bool CanEditAvatars { get; set; }

        public IEnumerable<GRA.Domain.Model.Branch> BranchList { get; set; }
        public IEnumerable<GRA.Domain.Model.System> SystemList { get; set; }
    }
}