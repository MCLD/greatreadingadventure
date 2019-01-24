using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Home
{
    public class AtAGlanceViewModel
    {
        public IEnumerable<NewsPost> NewsPosts { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public AtAGlanceReport AtAGlanceReport { get; set; }
        public bool IsNewsSubscribed { get; set; }
        public string SiteAdministratorEmail { get; set; }
        public int LatestNewsId { get; set; }
    }
}
