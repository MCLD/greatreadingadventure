using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HistoryItemViewModel
    {
        public string AttachmentDownload { get; set; }
        public string AttachmentFilename { get; set; }
        public int AttachmentId { get; set; }
        [MaxLength(255)]
        public string BadgeAltText { get; set; }

        public string BadgeFilename { get; set; }
        public string CreatedAt { get; set; }
        public string Description { get; set; }
        public int PointsEarned { get; set; }
        public bool ShowCertificate { get; set; }
    }
}
