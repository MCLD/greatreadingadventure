using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Programs
{
    public class PointTranslationsListViewModel
    {
        public ICollection<PointTranslation> PointTranslations { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public PointTranslation PointTranslation { get; set; }
    }
}
