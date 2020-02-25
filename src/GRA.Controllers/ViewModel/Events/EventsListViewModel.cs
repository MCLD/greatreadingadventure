using System;
using System.Collections.Generic;
using System.ComponentModel;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Events
{
    public class EventsListViewModel
    {
        public IList<GRA.Domain.Model.Event> Events { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Sort { get; set; }
        public bool? UseLocation { get; set; }
        public bool CommunityExperiences { get; set; }
        public bool ShowNearSearch { get; set; }
        public string CommunityExperienceDescription { get; set; }
        public string UserZipCode { get; set; }

        [DisplayName(DisplayNames.SearchTitleAndDescription)]
        public string Search { get; set; }

        [DisplayName(DisplayNames.NearLocation)]
        public string Near { get; set; }

        [DisplayName(DisplayNames.System)]
        public int? SystemId { get; set; }

        [DisplayName(DisplayNames.Branch)]
        public int? BranchId { get; set; }

        [DisplayName(DisplayNames.Location)]
        public int? LocationId { get; set; }

        [DisplayName(DisplayNames.Program)]
        public int? ProgramId { get; set; }

        [DisplayName(DisplayNames.StartDate)]
        public DateTime? StartDate { get; set; }

        [DisplayName(DisplayNames.EndDate)]
        public DateTime? EndDate { get; set; }

        public bool? Favorites { get; set; }

        [DisplayName(DisplayNames.Visited)]
        public string Visited { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList LocationList { get; set; }
        public SelectList ProgramList { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
