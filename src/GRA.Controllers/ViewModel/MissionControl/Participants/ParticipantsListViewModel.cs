﻿using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class ParticipantsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.User> Users { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public string Sort { get; set; }
        public bool IsDescending { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }
        public string ActiveNav { get; set; }
        public string SystemName { get; set; }
        public string BranchName { get; set; }
        public string ProgramName { get; set; }
        public bool CanRemoveParticipant { get; set; }
        public bool CanSignUpParticipants { get; set; }
        public bool CanViewDetails { get; set; }
        public bool ShowGroupsButton { get; set; }
        public System.Array SortUsers { get; set; }

        public IEnumerable<GRA.Domain.Model.Branch> BranchList { get; set; }
        public IEnumerable<GRA.Domain.Model.System> SystemList { get; set; }
        public IEnumerable<GRA.Domain.Model.Program> ProgramList { get; set; }
    }
}
