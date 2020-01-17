using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Lookup
{
    public class RequirementListViewModel
    {
        public IEnumerable<TriggerRequirement> Requirements { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
