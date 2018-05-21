using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteSocialMediaViewModel
    {
        public int Id { get; set; }

        [DisplayName("Facebook App ID")]
        [MaxLength(100)]
        public string FacebookAppId { get; set; }
        [DisplayName("Facebook/Open Graph Image URL")]
        [MaxLength(255)]
        public string FacebookImageUrl { get; set; }
        [DisplayName("Twitter Large Card")]
        public bool? TwitterLargeCard { get; set; }
        [DisplayName("Twitter Card Image URL")]
        [MaxLength(255)]
        public string TwitterCardImageUrl { get; set; }
        [DisplayName("Twitter Username")]
        [MaxLength(15)]
        public string TwitterUsername { get; set; }
        [DisplayName("Twitter Avatar Message")]
        [MaxLength(255)]
        public string TwitterAvatarMessage { get; set; }
        [DisplayName("Twitter Avatar Hashtag")]
        [MaxLength(100)]
        public string TwitterAvatarHashtags { get; set; }
        [DisplayName("Avatar Card Description")]
        [MaxLength(150)]
        public string AvatarCardDescription { get; set; }
    }
}
