using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Challenges
{
    public class ChallengeDetailViewModel
    {
        public GRA.Domain.Model.Challenge Challenge { get; set; }
        public bool IsActive { get; set; }
        public bool ShowCompleted { get; set; }
        public string Details { get; set; }
        public string BadgePath { get; set; }
        public List<TaskDetailViewModel> Tasks { get; set; }
    }
}
