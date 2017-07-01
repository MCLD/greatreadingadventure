using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Lookup
{
    public class RequirementListViewModel
    {
        public IEnumerable<TriggerRequirement> Requirements { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
