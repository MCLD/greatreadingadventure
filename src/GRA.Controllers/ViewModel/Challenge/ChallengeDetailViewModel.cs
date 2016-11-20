using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Challenge
{
    public class ChallengeDetailViewModel
    {
        public GRA.Domain.Model.Challenge Challenge { get; set; }
        public GRA.Domain.Model.ChallengeTask Task { get; set; }
        public GRA.Domain.Model.ChallengeTaskType TaskTypes { get; set; }
    }
}
