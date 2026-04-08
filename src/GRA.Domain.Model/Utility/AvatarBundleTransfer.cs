using System.Collections.Generic;

namespace GRA.Domain.Model.Utility
{
    public class AvatarBundleTransfer
    {
        public ICollection<AvatarBundleItemTransfer> AvatarItems { get; set; }
        public string Name { get; set; }
    }
}
