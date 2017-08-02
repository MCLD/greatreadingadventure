using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace GRA.Controllers.ViewModel.MissionControl.Schools
{
    public class EnteredListViewModel
    {
        public List<GRA.Domain.Model.EnteredSchool> EnteredSchools { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool NewSchool { get; set; }
        public GRA.Domain.Model.School EnteredSchool { get; set; }
        [DisplayName("School")]
        public int SchoolId { get; set; }
        public string Search { get; set; }
        public SelectList SchoolDistricts { get; set; }
        public SelectList SchoolTypes { get; set; }
        public int? CurrentPage { get; set; }
    }
}
