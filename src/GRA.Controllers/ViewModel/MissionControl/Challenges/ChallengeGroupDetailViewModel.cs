using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class ChallengeGroupDetailViewModel
    {
        public GRA.Domain.Model.ChallengeGroup ChallengeGroup { get; set; }
        public string ChallengeIds { get; set; }
        public string Action { get; set; }
        public List<Event> RelatedEvents { get; set; }
        public bool CanManageEvents { get; set; }
    }
}
