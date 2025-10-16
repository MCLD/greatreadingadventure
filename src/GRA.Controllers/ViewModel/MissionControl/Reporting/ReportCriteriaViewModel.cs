using System;
using System.Collections.Generic;
using System.ComponentModel;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportCriteriaViewModel
    {
        public string BadgeRequiredList { get; set; }

        [DisplayName("Select a branch")]
        public int? BranchId { get; set; }

        public SelectList BranchList { get; set; }
        public string ChallengeRequiredList { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Select a group")]
        public int? GroupInfoId { get; set; }

        public SelectList GroupInfosList { get; set; }
        public bool IncludeAchieverStatus { get; set; }
        public SelectList PrizeList { get; set; }

        [DisplayName("Select a program")]
        public int? ProgramId { get; set; }

        public SelectList ProgramList { get; set; }
        public DateTime ProgramStartDate { get; set; }

        [DisplayName("Badges to Report")]
        public ICollection<TriggerRequirement> ReportBadges { get; set; }

        public int ReportId { get; set; }

        [DisplayName("Select a school district")]
        public int? SchoolDistrictId { get; set; }

        public SelectList SchoolDistrictList { get; set; }

        [DisplayName("Select a school")]
        public int? SchoolId { get; set; }

        public SelectList SchoolList { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("Select a system")]
        public int? SystemId { get; set; }

        public SelectList SystemList { get; set; }

        [DisplayName("Select triggers (use SHIFT or CTRL to select multiple items)")]
        public List<int> TriggerList { get; set; }

        [DisplayName("Select a vendor code")]
        public int? VendorCodeTypeId { get; set; }

        public SelectList VendorCodeTypeList { get; set; }
    }
}
