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
        [DisplayName(DisplayNames.Branch)]
        public int? BranchId { get; set; }

        public SelectList BranchList { get; set; }
        public string CommunityExperienceDescription { get; set; }

        [DisplayName(DisplayNames.EndDate)]
        public DateTime? EndDate { get; set; }

        public IList<Event> Events { get; set; }
        public EventType EventType { get; set; }
        public bool? Favorites { get; set; }

        public string FormAction
        {
            get
            {
                return EventType switch
                {
                    EventType.CommunityExperience => nameof(EventsController.CommunityExperiences),
                    EventType.StreamingEvent => nameof(EventsController.StreamingEvents),
                    _ => nameof(EventsController.Index),
                };
            }
        }

        public bool IsAuthenticated { get; set; }
        public bool IsLoggedIn { get; set; }

        [DisplayName(DisplayNames.Location)]
        public int? LocationId { get; set; }

        public SelectList LocationList { get; set; }

        [DisplayName(DisplayNames.NearLocation)]
        public string Near { get; set; }

        public PaginateViewModel PaginateModel { get; set; }

        [DisplayName(DisplayNames.Program)]
        public int? ProgramId { get; set; }

        public SelectList ProgramList { get; set; }

        [DisplayName(DisplayNames.SearchTitleAndDescription)]
        public string Search { get; set; }

        public bool ShowNearSearch { get; set; }
        public string Sort { get; set; }

        [DisplayName(DisplayNames.StartDate)]
        public DateTime? StartDate { get; set; }

        [DisplayName(DisplayNames.System)]
        public int? SystemId { get; set; }

        public SelectList SystemList { get; set; }
        public bool? UseLocation { get; set; }
        public string UserZipCode { get; set; }

        [DisplayName(DisplayNames.Viewed)]
        public string Viewed { get; set; }

        [DisplayName(DisplayNames.Visited)]
        public string Visited { get; set; }
    }
}
