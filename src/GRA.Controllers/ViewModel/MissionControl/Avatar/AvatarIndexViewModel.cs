using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.Avatar
{
    public class AvatarIndexViewModel
    {
        public bool DefaultAvatarsPresent { get; set; }
        public bool AvatarZipPresent { get; set; }
        public IEnumerable<Domain.Model.AvatarLayer> Layers { get; set; }
        public IFormFile UploadedFile { get; set; }
    }
}
