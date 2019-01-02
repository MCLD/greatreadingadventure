using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class KitImagesViewModel
    {
        public int KitId { get; set; }
        public string KitName { get; set; }
        public List<PsKitImage> KitImages { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImagesToDelete { get; set; }
        public int MaxUploadMB { get; set; }
    }
}
