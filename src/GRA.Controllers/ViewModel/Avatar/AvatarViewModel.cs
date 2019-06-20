using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class AvatarViewModel
    {
        public List<List<AvatarLayer>> LayerGroupings { get; set; }
        public List<AvatarBundle> Bundles { get; set; }
        public List<string> ViewedBundles { get; set; }
        public int DefaultLayer { get; set; }
        public string ImagePath { get; set; }
        public string AvatarPiecesJson { get; set; }
        public string AvatarBundlesJson { get; set; }
        public bool NewAvatar { get; set; }
    }
}
