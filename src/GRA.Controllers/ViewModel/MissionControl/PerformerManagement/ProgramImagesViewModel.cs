using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class ProgramImagesViewModel
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public List<PsProgramImage> ProgramImages { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImagesToDelete { get; set; }
        public int MaxUploadMB { get; set; }
    }
}
