using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ExitLandingMessageSet : Abstract.BaseDbEntity
    {
        [Required]
        public int BannerAlt { get; set; }

        [Required]
        public int BannerFile { get; set; }

        [Required]
        public int ExitLeftMessage { get; set; }

        [Required]
        public int LandingCenterMessage { get; set; }

        [Required]
        public int LandingLeftMessage { get; set; }

        [Required]
        public int LandingRightMessage { get; set; }
    }
}
