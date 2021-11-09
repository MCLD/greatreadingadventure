using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class PreconfiguredAvatarsViewModel
    {
        public int[] SelectedItemIds { get; set; }
        public string ItemPath { get; internal set; }
        public AvatarBundle Bundle { get; set; }
    }
}
