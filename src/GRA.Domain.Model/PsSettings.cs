using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsSettings : Abstract.BaseDomainEntity
    {
        [DisplayName("Branch Availability Supplemental Text")]
        [MaxLength(255)]
        public string BranchAvailabilitySupplementalText { get; set; }

        [DisplayName("Contact Email")]
        [MaxLength(255)]
        public string ContactEmail { get; set; }

        [DisplayName("Registration Closed (remember to set a date and a time)")]
        public DateTime? RegistrationClosed { get; set; }

        [DisplayName("Registration Open (remember to set a date and a time)")]
        public DateTime? RegistrationOpen { get; set; }

        [DisplayName("Schedule End Date (remember to set a date and a time)")]
        public DateTime? ScheduleEndDate { get; set; }

        [DisplayName("Schedule Posted (remember to set a date and a time)")]
        public DateTime? SchedulePosted { get; set; }

        [DisplayName("Schedule Start Date (remember to set a date and a time)")]
        public DateTime? ScheduleStartDate { get; set; }

        [DisplayName("Scheduling Closed (remember to set a date and a time)")]
        public DateTime? SchedulingClosed { get; set; }

        [DisplayName("Scheduling Open (remember to set a date and a time)")]
        public DateTime? SchedulingOpen { get; set; }

        [DisplayName("Scheduling Preview (remember to set a date and a time)")]
        public DateTime? SchedulingPreview { get; set; }

        [DisplayName("Selections Per Branch")]
        [Range(1, int.MaxValue)]
        public int? SelectionsPerBranch { get; set; }

        [DisplayName("Program Set up Supplemental Text")]
        [MaxLength(50)]
        public string SetupSupplementalText { get; set; }

        public int SiteId { get; set; }

        [DisplayName("Vendor Code Format")]
        [MaxLength(255)]
        public string VendorCodeFormat { get; set; }

        [DisplayName("Vendor ID Prompt")]
        [MaxLength(255)]
        public string VendorIdPrompt { get; set; }

        [DisplayName("Cover Sheet Staff Contact")]
        [MaxLength(255)]
        public string CoverSheetContact { get; set; }

        [DisplayName("Cover Sheet Library Branch")]
        [MaxLength(255)]
        public string CoverSheetBranch { get; set; }
    }
}
