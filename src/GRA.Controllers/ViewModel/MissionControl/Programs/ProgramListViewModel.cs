using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Programs
{
    public class ProgramListViewModel
    {
        public ICollection<Program> Programs { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public Program Program { get; set; }
    }
}
