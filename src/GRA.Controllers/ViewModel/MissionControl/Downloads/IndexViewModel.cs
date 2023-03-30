using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Downloads
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            SystemBadgeCount = new Dictionary<Domain.Model.System, int>();
        }

        public IDictionary<Domain.Model.System, int> SystemBadgeCount { get; }
    }
}
