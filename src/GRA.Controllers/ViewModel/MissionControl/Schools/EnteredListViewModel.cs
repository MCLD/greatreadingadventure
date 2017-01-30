using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Schools
{
    public class EnteredListViewModel
    {
        public List<GRA.Domain.Model.EnteredSchool> EnteredSchools { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.School EnteredSchool { get; set; }
        public SelectList SchoolDistricts { get; set; }
        public SelectList SchoolTypes { get; set; }
    }
}
