using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Dashboard
{
    public class DashboardListViewModel
    {
        public IList<GRA.Domain.Model.DashboardContent> DashboardContents { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int DashboardContentId { get; set; }
        public bool IsArchived { get; set; }
        public bool HighlightFirst { get; set; }
    }
}
