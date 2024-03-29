﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Program : Abstract.BaseDomainEntity
    {
        [DisplayName("Achiever Point Amount")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        [Required]
        public int AchieverPointAmount { get; set; }

        [DisplayName("Maximum Age")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int? AgeMaximum { get; set; }

        [DisplayName("Minimum Age")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int? AgeMinimum { get; set; }

        [Required]
        public bool AgeRequired { get; set; }

        [Required]
        public bool AskAge { get; set; }

        [Required]
        public bool AskSchool { get; set; }

        public DailyLiteracyTip DailyLiteracyTip { get; set; }

        [DisplayName("Daily Literacy Tip")]
        public int? DailyLiteracyTipId { get; set; }

        [MaxLength(255)]
        [DisplayName("Dashboard Alert")]
        public string DashboardAlert { get; set; }

        public AlertType DashboardAlertType { get; set; }

        [DisplayName("Badge Alternative Text")]
        [MaxLength(255)]
        public string JoinBadgeAltText { get; set; }

        public int? JoinBadgeId { get; set; }

        [DisplayName("Badge Name")]
        [MaxLength(255)]
        public string JoinBadgeName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public PointTranslation PointTranslation { get; set; }

        [DisplayName("Point Translation")]
        public int PointTranslationId { get; set; }

        public int Position { get; set; }

        [Required]
        public bool SchoolRequired { get; set; }

        [Required]
        public int SiteId { get; set; }
    }
}
