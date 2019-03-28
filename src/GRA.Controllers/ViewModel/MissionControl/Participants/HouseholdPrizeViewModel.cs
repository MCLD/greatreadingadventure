using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HouseholdPrizeViewModel
    {
        public ICollection<User> Users { get; set; }
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int? DrawingId { get; set; }
        public int? TriggerId { get; set; }
        public string PrizeName { get; set; }
        public string UserSelection { get; set; }
    }
}
