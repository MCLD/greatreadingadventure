﻿using System;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class PerformerViewModel
    {
        public PsPerformer Performer { get; set; }
        public PsSettings Settings { get; set; }
        public PsSchedulingStage SchedulingStage { get; set; }
        public IEnumerable<Domain.Model.System> Systems { get; set; }
        public string ImagePath { get; set; }
        public string ReferencesPath { get; set; }
        public Uri Uri { get; set; }
        public PsProgram ProgramToDelete { get; set; }
        public bool Approve { get; set; }

        public int? NextPerformer { get; set; }
        public int? PrevPerformer { get; set; }
        public int ReturnPage { get; set; }

        public List<int> BranchAvailability { get; set; }
        public ICollection<PsBlackoutDate> BlackoutDates { get; set; }
    }
}
