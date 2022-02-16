using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using static GRA.Domain.Model.Filters.ChallengeFilter;

namespace GRA.Controllers.ViewModel.Challenges
{
    public class ChallengesListViewModel
    {
        public string Categories { get; set; }
        public IEnumerable<int> CategoryIds { get; set; }
        public SelectList CategoryList { get; set; }
        public GRA.Domain.Model.ChallengeGroup ChallengeGroup { get; set; }
        public IList<GRA.Domain.Model.Challenge> Challenges { get; set; }
        public bool? Favorites { get; set; }
        public bool IsActive { get; set; }
        public bool IsLoggedIn { get; set; }
        public OrderingOption Ordering { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int? Program { get; set; }
        public SelectList ProgramList { get; set; }
        public string Search { get; set; }
        public string Status { get; set; }

        public string IsChecked(OrderingOption orderingOption)
        {
            return orderingOption == Ordering ? "checked" : null;
        }
    }
}
