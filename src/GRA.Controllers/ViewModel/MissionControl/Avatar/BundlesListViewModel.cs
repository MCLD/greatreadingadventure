using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class BundlesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.DynamicAvatarBundle> Bundles { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool Unlockable { get; set; }
    }
}
