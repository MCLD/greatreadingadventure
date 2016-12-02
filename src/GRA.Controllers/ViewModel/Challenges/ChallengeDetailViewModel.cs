using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Challenges
{
    public class ChallengeDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string BadgePath { get; set; }
        public List<TaskDetailViewModel> Tasks { get; set; }
    }
}
