using System;
using System.Collections.Generic;
using System.ComponentModel;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportCriteriaViewModel
    {
        public int ReportId { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        public DateTime ProgramStartDate { get; set; }

        [DisplayName("Select a system")]
        public int? SystemId { get; set; }

        [DisplayName("Select a branch")]
        public int? BranchId { get; set; }

        [DisplayName("Select a program")]
        public int? ProgramId { get; set; }

        [DisplayName("Select a school district")]
        public int? SchoolDistrictId { get; set; }

        [DisplayName("Select a school")]
        public int? SchoolId { get; set; }

        [DisplayName("Select a group")]
        public int? GroupInfoId { get; set; }

        [DisplayName("Select a vendor code")]
        public int? VendorCodeTypeId { get; set; }

        [DisplayName("Select triggers")]
        public List<int> TriggerList { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SchoolDistrictList { get; set; }
        public SelectList SchoolList { get; set; }
        public SelectList GroupInfosList { get; set; }
        public SelectList VendorCodeTypeList { get; set; }
        public SelectList PrizeList { get; set; }

        [DisplayName("Badges to Report")]
        public ICollection<TriggerRequirement> ReportBadges { get; set; }

        public string BadgeRequiredList { get; set; }
        public string ChallengeRequiredList { get; set; }
    }
}
