using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string ItemIds { get; set; }
        public List<int> SelectedItemIds { get; set; }
        public ICollection<AvatarItem> SelectedItems { get; set; }
        [Required]
        public int AssociatedBundleId { get; set; }
    }
}
