using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Shared
{
    public class DisplayNotificationsViewModel
    {
        public List<GRA.Domain.Model.Notification> Notifications { get; set; }
        public string SummaryText { get; set; }
    }
}
