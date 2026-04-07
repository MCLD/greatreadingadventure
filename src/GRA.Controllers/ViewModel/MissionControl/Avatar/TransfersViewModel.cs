using System.Collections.Generic;
using System.Globalization;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class TransfersViewModel
    {
        public TransfersViewModel()
        {
            Transfers = [];
            Jobs = new Dictionary<int, Job>();
        }

        public bool AvatarZipPresent { get; set; }
        public bool DefaultAvatarsPresent { get; set; }
        public IDictionary<int, Job> Jobs { get; }
        public bool LayersPresent { get; set; }
        public ICollection<AvatarTransfer> Transfers { get; }
        public IFormFile UploadedFile { get; set; }

        public static string FormatDuration(Job job)
        {
            return job?.Duration.HasValue == true
                ? job.Duration.Value.ToString(@"m\m\ s\s", CultureInfo.InvariantCulture)
                : null;
        }

        public static string OutputSize(long kBytes)
        {
            return $"({kBytes:N0}KB)";
        }
    }
}
