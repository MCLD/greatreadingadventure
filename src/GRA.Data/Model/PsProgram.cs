using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsProgram : Abstract.BaseDbEntity
    {
        [Required]
        public int PerformerId { get; set; }
        public PsPerformer Performer { get; set; }

        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        [Range(1, int.MaxValue)]
        public int MinimumCapacity { get; set; }

        [Range(1, int.MaxValue)]
        public int MaximumCapacity { get; set; }

        public int ProgramLengthMinutes { get; set; }

        public int SetupTimeMinutes { get; set; }

        public int BreakdownTimeMinutes { get; set; }

        public int BackToBackMinutes { get; set; }

        [MaxLength(375)]
        [Required]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [MaxLength(1000)]

        public string Setup { get; set; }

        public bool AllowStreaming { get; set; }

        public bool AllowArchiving { get; set; }

        public ICollection<PsProgramAgeGroup> AgeGroups { get; set; }
        public IList<PsProgramImage> ProgramImages { get; set; }
    }
}
