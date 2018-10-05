using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsProgram : Abstract.BaseDomainEntity
    {
        [Required]
        public int PerformerId { get; set; }
        public PsPerformer Performer { get; set; }

        [DisplayName("Program name")]
        [MaxLength(50, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        [Required]
        public string Title { get; set; }

        [DisplayName("Minimum participants")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a number larger than 0.")]
        public int MinimumCapacity { get; set; }

        [DisplayName("Maximum participants")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a number larger than 0.")]
        public int MaximumCapacity { get; set; }

        [DisplayName("Program length")]
        public int ProgramLengthMinutes { get; set; }

        [DisplayName("Set up time")]
        public int SetupTimeMinutes { get; set; }

        [DisplayName("Breakdown time")]
        public int BreakdownTimeMinutes { get; set; }

        [DisplayName("Time between back-to-back programs")]
        public int BackToBackMinutes { get; set; }

        [DisplayName("Program description")]
        [MaxLength(375, ErrorMessage = "Please enter no more than {1} characters for {0}.")]
        [Required]
        public string Description { get; set; }

        [DisplayName("Inclusive cost")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a non-negative cost.")]
        public decimal Cost { get; set; }

        [DisplayName("Library set up")]
        [MaxLength(1000, ErrorMessage = "Please enter no more than {1} characters for {0}.")]

        public string Setup { get; set; }

        [DisplayName("Allow streaming")]
        public bool AllowStreaming { get; set; }

        [DisplayName("Allow archiving")]
        public bool AllowArchiving { get; set; }

        public ICollection<PsAgeGroup> AgeGroups { get; set; }
        public IList<PsProgramImage> ProgramImages { get; set; }
    }
}
