using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Home
{
    public class AtAGlanceViewModel
    {
        public AtAGlanceReport AtAGlanceReport { get; set; }
        public int? Category { get; set; }
        public bool IsNewsSubscribed { get; set; }
        public int LatestNewsId { get; set; }
        public IEnumerable<NewsCategory> NewsCategories { get; set; }
        public IEnumerable<NewsPost> NewsPosts { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool ShowPosts { get; set; }
        public string SiteAdministratorEmail { get; set; }
        public List<bool> WithinAWeek { get; set; }
    }
}
