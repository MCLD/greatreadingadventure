using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class PreconfiguredDetailsViewModel
    {
        public List<List<AvatarLayer>> LayerGroupings { get; set; }
        public ICollection<AvatarBundle> Bundles { get; set; }
        public AvatarBundle Bundle { get; set; }
        public string ImagePath { get; set; }
        public bool NewAvatar { get; set; }
        public List<int> SelectedItemIds { get; set; }
        public ICollection<AvatarItem> SelectedItems { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int AssociatedBundleId { get; set; }
        public int BundleId { get; set; }
    }
}
