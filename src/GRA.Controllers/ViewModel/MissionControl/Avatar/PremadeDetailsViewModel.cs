using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class PremadeDetailsViewModel
    {
        public List<List<AvatarLayer>> LayerGroupings { get; set; }
        public ICollection<AvatarBundle> Bundles { get; set; }
        public AvatarBundle Bundle { get; set; }
        public int DefaultLayer { get; set; }
        public string ImagePath { get; set; }
        public bool NewAvatar { get; set; }
        public ICollection<AvatarItem> LayerItems { get; set; }
        public ICollection<AvatarColor> LayerColors { get; set; }
        public List<int> SelectedItemIds { get; set; }
        public ICollection<AvatarItem> SelectedItems { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int AssociatedBundleId { get; set; }
    }
}
