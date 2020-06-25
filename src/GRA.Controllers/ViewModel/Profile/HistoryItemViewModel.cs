using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HistoryItemViewModel
    {
        public string CreatedAt { get; set; }
        public string Description { get; set; }
        public int PointsEarned { get; set; }
        public string BadgeFilename { get; set; }

        [MaxLength(255)]
        public string BadgeAltText { get; set; }
    }
}
