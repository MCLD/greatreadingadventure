using System.Collections.Generic;
using System.Globalization;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class TransfersViewModel
    {
        public TransfersViewModel()
        {
            Transfers = [];
            Jobs = new Dictionary<int, Job>();
        }

        public ICollection<AvatarTransfer> Transfers { get; }
        public IDictionary<int, Job> Jobs { get; }

        public static string FormatDuration(Job job)
        {
            return job?.Duration.HasValue == true
                ? job.Duration.Value.ToString(@"m\m\ s\s", CultureInfo.InvariantCulture)
                : null;
        }
    }
}
