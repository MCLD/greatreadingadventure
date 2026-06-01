using System;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class ShareViewModel
    {
        public string AvatarId { get; set; }
        public Uri AvatarImageUrl { get; set; }
        public Uri FacebookShareUrl { get; set; }
        public Uri TwitterShareUrl { get; set; }
    }
}
