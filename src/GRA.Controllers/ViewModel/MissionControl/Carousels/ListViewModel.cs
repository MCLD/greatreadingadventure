using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Carousels
{
    public class ListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Carousel> Carousels { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
