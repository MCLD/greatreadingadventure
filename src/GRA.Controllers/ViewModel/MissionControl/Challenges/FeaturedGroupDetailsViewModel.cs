using System.ComponentModel;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class FeaturedGroupDetailsViewModel
    {
        public FeaturedChallengeGroup FeaturedGroup { get; set; }
        public FeaturedChallengeGroupText FeaturedGroupText { get; set; }
        public bool NewFeaturedGroup { get; set; }

        public SelectList ChallengeGroupList { get; set; }

        [DisplayName("Image file")]
        public IFormFile UploadedImage { get; set; }

        public int FeaturedGroupId { get; set; }
    }
}
