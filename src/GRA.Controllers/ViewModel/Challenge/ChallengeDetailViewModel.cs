using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Challenge
{
    public class ChallengeDetailViewModel
    {
        public GRA.Domain.Model.Challenge Challenge { get; set; }
        public GRA.Domain.Model.ChallengeTask Task { get; set; }
        public List<SelectListItem> TaskTypes {get; set; }
        public bool AddTask { get; set; }
    }
}