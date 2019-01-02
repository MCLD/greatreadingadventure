using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class ProgramImagesViewModel
    {
        public bool IsEditable { get; set; }
        public int ProgramId { get; set; }
        public string ProgramTitle { get; set; }
        public List<PsProgramImage> ProgramImages { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImagesToDelete { get; set; }
        public int MaxUploadMB { get; set; }
    }
}
