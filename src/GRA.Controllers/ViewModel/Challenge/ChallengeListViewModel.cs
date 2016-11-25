using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Challenge
{
    public class ChallengeListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Challenge> Challenges { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public string FilterBy { get; set; }
        public int AllCount { get; set; }
        public int SystemCount { get; set; }
        public int BranchCount { get; set; }
        public int MineCount { get; set; }
    }
}
