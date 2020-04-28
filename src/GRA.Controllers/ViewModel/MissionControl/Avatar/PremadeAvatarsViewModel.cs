using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class PremadeAvatarsViewModel
    {
        public AvatarBundle PremadeAvatarBundle { get; set; }
        public int[] SelectedItemIds { get; set; }
        public string ImagePath { get; set; }
        public string ItemPath { get; internal set; }
        public int LayerId { get; set; }
        public int SelectedItemId { get; set; }
        public ICollection<AvatarItem> LayerItems { get; set; }
        public ICollection<AvatarColor> LayerColors { get; set; }
        public AvatarBundle Bundle { get; set; }
    }
}
