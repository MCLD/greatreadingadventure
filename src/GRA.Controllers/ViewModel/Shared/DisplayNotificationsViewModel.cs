using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Shared
{
    public class DisplayNotificationsViewModel
    {
        public List<GRA.Domain.Model.Notification> Notifications { get; set; }
        public string SummaryText { get; set; }
    }
}
