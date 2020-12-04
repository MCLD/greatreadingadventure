using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Events
{
    public class LocationsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Location> Locations { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public GRA.Domain.Model.Location Location { get; set; }
        public bool ShowGeolocation { get; set; }
        public string GoogleMapsAPIKey { get; set; }
    }
}
