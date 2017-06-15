
namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HistoryItemViewModel
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public string Description { get; set; }
        public string ItemName { get; set; }
        public int PointsEarned { get; set; }
        public string BadgeFilename { get; set; }
        public bool IsDeletable { get; set; }
    }
}
