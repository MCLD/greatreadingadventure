using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsSettings : Abstract.BaseDbEntity
    {
        public int SiteId { get; set; }
        public Site Site { get; set; }

        [MaxLength(255)]
        public string ContactEmail { get; set; }

        public int? SelectionsPerBranch { get; set; }

        public DateTime? RegistrationOpen { get; set; }
        public DateTime? RegistrationClosed { get; set; }
        public DateTime? SchedulingPreview { get; set; }
        public DateTime? SchedulingOpen { get; set; }
        public DateTime? SchedulingClosed { get; set; }
        public DateTime? SchedulePosted { get; set; }

        public DateTime? ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }

        [MaxLength(255)]
        public string VendorIdPrompt { get; set; }

        [MaxLength(255)]
        public string VendorCodeFormat { get; set; }

        [MaxLength(255)]
        public string BranchAvailabilitySupplementalText { get; set; }

        [MaxLength(50)]
        public string SetupSupplementalText { get; set; }

        [MaxLength(255)]
        public string CoverSheetContact { get; set; }

        [MaxLength(255)]
        public string CoverSheetBranch { get; set; }
    }
}
