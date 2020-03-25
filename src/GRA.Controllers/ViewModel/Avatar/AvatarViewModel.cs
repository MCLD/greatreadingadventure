using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class AvatarViewModel
    {
        public List<List<AvatarLayer>> LayerGroupings { get; set; }
        public List<AvatarBundle> Bundles { get; set; }
        public AvatarBundle Bundle { get; set; }
        public List<bool> ViewedBundles { get; set; }
        public int DefaultLayer { get; set; }
        public string ImagePath { get; set; }
        public string AvatarPiecesJson { get; set; }
        public string AvatarBundlesJson { get; set; }
        public bool NewAvatar { get; set; }
        public bool HasUnviewedBundles { get; set; }
        public ICollection<AvatarItem> LayerItems{get; set;}
        public ICollection<AvatarColor> LayerColors { get; set; }
        public int SelectedItemId { get; set; }
        public int[] SelectedItemIds { get; set; }
        public string ItemPath { get; set; }
        public int LayerId { get; set; }
        public bool UseEnglish { get; set; }
    }
}
