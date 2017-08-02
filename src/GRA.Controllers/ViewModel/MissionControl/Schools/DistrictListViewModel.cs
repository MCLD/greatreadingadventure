using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Schools
{
    public class DistrictListViewModel
    {
        public List<GRA.Domain.Model.SchoolDistrict> SchoolDistricts { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.SchoolDistrict District { get; set; }
        public string Search { get; set; }
    }
}
