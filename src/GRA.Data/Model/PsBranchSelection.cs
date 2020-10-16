﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsBranchSelection : Abstract.BaseDbEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public int AgeGroupId { get; set; }
        public PsAgeGroup AgeGroup { get; set; }

        public int? ProgramId { get; set; }
        public PsProgram Program { get; set; }

        public int? KitId { get; set; }
        public PsKit Kit { get; set; }

        public DateTime RequestedStartTime { get; set; }
        public DateTime ScheduleStartTime { get; set; }
        public int ScheduleDuration { get; set; }
        public bool BackToBackProgram { get; set; }

        public DateTime SelectedAt { get; set; }

        [MaxLength(50)]
        public string SecretCode { get; set; }

        public bool IsDeleted { get; set; }
    }
}
