using GRA.Controllers.ViewModel.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Challenges
{
    public class ChallengesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Challenge> Challenges { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
    }
}
