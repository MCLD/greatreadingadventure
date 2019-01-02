using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerScheduling
{
    public class ProgramListViewModel
    {
        public ICollection<PsProgram> Programs { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public ICollection<PsAgeGroup> AgeGroups { get; set; }
        public int? AgeGroupId { get; set; }
    }
}
