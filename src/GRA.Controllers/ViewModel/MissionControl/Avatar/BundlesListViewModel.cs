using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class BundlesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.AvatarBundle> Bundles { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool? Unlockable { get; set; }
        public string Search { get; set; }

    }
}
