using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class AvatarBundleJsonModel
    {
        public ICollection<AvatarBundle> Bundles { get; set; }

        public class AvatarBundle
        {
            public int Id { get; set; }
            public ICollection<int> Items { get; set; }
        }

    }
}
