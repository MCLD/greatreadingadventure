using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class ChallengesDetailViewModel
    {
        public GRA.Domain.Model.Challenge Challenge { get; set; }
        public GRA.Domain.Model.ChallengeTask Task { get; set; }
        public List<SelectListItem> TaskTypes {get; set; }
        public bool AddTask { get; set; }
        public string BadgePath { get; set; }
        [DisplayName("Image")]
        public IFormFile BadgeImage { get; set; }
    }
}