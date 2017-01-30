using GRA.Controllers.ViewModel.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Schools
{
    public class DistrictListViewModel
    {
        public List<GRA.Domain.Model.SchoolDistrict> SchoolDistricts { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.SchoolDistrict District { get; set; }
    }
}
