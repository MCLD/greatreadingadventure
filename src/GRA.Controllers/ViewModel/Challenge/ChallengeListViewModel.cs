using GRA.Controllers.ViewModel.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
