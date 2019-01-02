using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class PerformerImagesViewModel
    {
        public int PerformerId { get; set; }
        public string PerformerName { get; set; }
        public List<PsPerformerImage> PerformerImages { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImagesToDelete { get; set; }
        public int MaxUploadMB { get; set; }
    }
}
