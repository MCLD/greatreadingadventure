using System;
using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class FeaturedGroupListViewModel
    {
        public IEnumerable<FeaturedChallengeGroup> FeaturedGroups { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        
        public FeaturedChallengeGroup FeaturedGroup { get; set; }

        public bool CanManageFeaturedGroups { get; set; }

        public bool ShowActive { get; set; }

        public DateTime Now { get; set; }
    }
}
