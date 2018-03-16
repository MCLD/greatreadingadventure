using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteSocialMediaViewModel
    {
        public int Id { get; set; }

        [DisplayName("Facebook App Id")]
        [MaxLength(100)]
        public string FacebookAppId { get; set; }
        [DisplayName("Facebook Image Url")]
        [MaxLength(100)]
        public string FacebookImageUrl { get; set; }
        [DisplayName("Twitter Large Card")]
        public bool? TwitterLargeCard { get; set; }
        [DisplayName("Twitter Card ImageUrl")]
        [MaxLength(100)]
        public string TwitterCardImageUrl { get; set; }
        [DisplayName("Twitter Username")]
        [MaxLength(15)]
        public string TwitterUsername { get; set; }
    }
}
