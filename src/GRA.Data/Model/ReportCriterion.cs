﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ReportCriterion : Abstract.BaseDbEntity
    {
        public int SiteId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }
        public int? SchoolDistrictId { get; set; }
        public int? SchoolId { get; set; }
        public int? GroupInfoId { get; set; }
        public int? VendorCodeTypeId { get; set; }

        public bool Favorite { get; set; }

        public int? MaximumAllowableActivity { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public string BadgeRequiredList { get; set; }
        public string ChallengeRequiredList { get; set; }
        public string TriggerList { get; set; }
    }
}
