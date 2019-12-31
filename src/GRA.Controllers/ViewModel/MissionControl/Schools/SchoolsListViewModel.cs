using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Schools
{
    public class SchoolsListViewModel
    {
        public List<GRA.Domain.Model.School> Schools { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.School School { get; set; }
        public string Search { get; set; }
        public ICollection<GRA.Domain.Model.SchoolDistrict> DistrictList { get; set; }
    }
}
