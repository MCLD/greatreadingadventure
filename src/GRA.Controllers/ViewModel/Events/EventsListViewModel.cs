using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GRA.Controllers.ViewModel.Events
{
    public class EventsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Event> Events { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        [DisplayName("System")]
        public int? SystemId { get; set; }
        [DisplayName("Branch")]
        public int? BranchId { get; set; }
        [DisplayName("Location")]
        public int? LocationId { get; set; }
        public bool? UseLocation { get; set; }
        [DisplayName("Program")]
        public int? ProgramId { get; set; }
        public string Search { get; set; }
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList LocationList { get; set; }
        public SelectList ProgramList { get; set; }
    }
}
