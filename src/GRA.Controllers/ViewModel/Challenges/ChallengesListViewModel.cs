using System.Collections.Generic;
using System.ComponentModel;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Challenges
{
    public class ChallengesListViewModel
    {
        public IList<GRA.Domain.Model.Challenge> Challenges { get; set; }
        public GRA.Domain.Model.ChallengeGroup ChallengeGroup { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public int? Program { get; set; }
        public string Categories { get; set; }
        public bool IsActive { get; set; }
        public bool IsLoggedIn { get; set; }

        [DisplayName(DisplayNames.FilterFavorites)]
        public int? FavoriteSelection { get; set; }

        public SelectList ProgramList { get; set; }
        public SelectList CategoryList { get; set; }
        public SelectList FavoriteList { get; set; }
        public IEnumerable<int> CategoryIds { get; set; }
    }
    public enum FilterFavorites
    {
        OnlyFavorites,
        HideFavorites
    }
}
